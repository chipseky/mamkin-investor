using Bybit.Net.Clients;
using CryptoExchange.Net.Authentication;
using Mamkin.In.Domain;
using Mamkin.In.Infrastructure.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderSide = Mamkin.In.Domain.OrderSide;

namespace Mamkin.In.Infrastructure.ApiClients;

public class FakeOrdersApi : IOrdersApi
{
    private readonly ILogger<BybitOrdersApiClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<BybitSettings> _bybitSettings;
    private readonly IDistributedCache _cache;

    public FakeOrdersApi(
        ILogger<BybitOrdersApiClient> logger,
        IHttpClientFactory httpClientFactory, 
        IOptions<BybitSettings> bybitSettings, 
        IDistributedCache cache)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _bybitSettings = bybitSettings;
        _cache = cache;
    }

    public async Task<PlaceOrderResult> PlaceBuyOrder(string symbol, decimal quantity, string clientOrderId)
    {
        try
        {
            var fakeOrderId = Guid.NewGuid();
            var fakeOrder = new FakeOrder(symbol, quantity, clientOrderId, OrderSide.Buy);

            await _cache.SetValue(
                key: fakeOrderId.ToString(), 
                value: fakeOrder, 
                options: new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }, 
                cancellationToken: CancellationToken.None);
            
            return PlaceOrderResult.Success(fakeOrderId.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "generating fake order error.");
            return PlaceOrderResult.Fail(ex.Message);
        }
    }

    public async Task<PlaceOrderResult> PlaceSellOrder(string symbol, decimal quantity, string clientOrderId)
    {
        try
        {
            var fakeOrderId = Guid.NewGuid();
            var fakeOrder = new FakeOrder(symbol, quantity, clientOrderId, OrderSide.Sell);

            await _cache.SetValue(
                key: fakeOrderId.ToString(), 
                value: fakeOrder, 
                options: new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }, 
                cancellationToken: CancellationToken.None);

            return PlaceOrderResult.Success(fakeOrderId.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "generating fake order error.");
            return PlaceOrderResult.Fail(ex.Message);
        }
    }

    public async Task<TradeOrder?> GetOrder(string fakeOrderId)
    {
        var fakeOrder = await _cache.GetValue<FakeOrder>(fakeOrderId, CancellationToken.None);
        if (fakeOrder != null)
        {
            var bybitClient = CreateBybitClient();
            var result = await bybitClient.V5Api.ExchangeData.GetSpotTickersAsync(fakeOrder.Symbol);

            if (result.Error != null)
                return null;
            
            var symbolInfo = result.Data.List.First();
            
            if(fakeOrder.OrderSide == OrderSide.Buy)
                return TradeOrder.Create(
                    tradeOrderId: fakeOrderId,
                    actualAveragePrice: symbolInfo.LastPrice,
                    quantity: fakeOrder.Quantity, // сколько хотел потратить usdt
                    quantityFilled: fakeOrder.Quantity / symbolInfo.LastPrice, // количество купленных монет
                    executedFee: fakeOrder.Quantity / symbolInfo.LastPrice * BybitCommissionConstants.BuyCommission,
                    valueFilled: fakeOrder.Quantity, // сколько по факту потратил
                    valueRemaining: 0, // из запланированного осталось
                    status: OrderStatus.PartiallyFilledCanceled,
                    orderSide: fakeOrder.OrderSide
                );
            else
                return TradeOrder.Create(
                    tradeOrderId: fakeOrderId,
                    actualAveragePrice: symbolInfo.LastPrice,
                    quantity: fakeOrder.Quantity, // сколько монет хотел продать
                    quantityFilled: fakeOrder.Quantity, // сколько монет продал по факту
                    executedFee: fakeOrder.Quantity * symbolInfo.LastPrice * BybitCommissionConstants.SellCommission,
                    valueFilled: fakeOrder.Quantity * symbolInfo.LastPrice, // сколько получил баксов за продажу монет, 0.997749349m and 0.002250651m just to emulate real case
                    valueRemaining: 0,
                    status: OrderStatus.PartiallyFilledCanceled,
                    orderSide: fakeOrder.OrderSide
                );
        }
        else
        {
            _logger.LogError("cannot find fake order with id {fakeOrderId}", fakeOrderId);
            return null;
        }
    }

    private BybitRestClient CreateBybitClient()
    {
        var bybitClient = new BybitRestClient(
            httpClient: _httpClientFactory.CreateClient("bybit_client"),
            optionsDelegate: options =>
            {
                options.ApiCredentials = new ApiCredentials(_bybitSettings.Value.ApiKey, _bybitSettings.Value.ApiSecret);
            },
            loggerFactory: null);
        
        return bybitClient;
    }
}

// ReSharper disable once NotAccessedPositionalProperty.Global
public record FakeOrder(string Symbol, decimal Quantity, string ClientOrderId, OrderSide OrderSide);