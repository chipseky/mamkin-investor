using Bybit.Net;
using Bybit.Net.Clients;
using Bybit.Net.Enums;
using CryptoExchange.Net.Authentication;
using Mamkin.In.Domain;
using Mamkin.In.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamkin.In.Infrastructure.ApiClients;

public class BybitOrdersApiClient : IOrdersApi
{
    private readonly ILogger<BybitOrdersApiClient> _logger;
    private readonly IOptions<BybitSettings> _bybitSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly LotSizeFilterService _lotSizeFilterService;

    public BybitOrdersApiClient(
        IOptions<BybitSettings> bybitSettings, 
        ILogger<BybitOrdersApiClient> logger,
        IHttpClientFactory httpClientFactory,
        LotSizeFilterService lotSizeFilterService)
    {
        _bybitSettings = bybitSettings;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _lotSizeFilterService = lotSizeFilterService;
    }

    public async Task<PlaceOrderResult> PlaceBuyOrder(string symbol, decimal quantity, string clientOrderId)
    {
        BybitRestClient? bybitClient = null;
        try
        {
            bybitClient = CreateBybitClient();

            var result = await bybitClient.V5Api.Trading.PlaceOrderAsync(
                category: Category.Spot,
                symbol: symbol,
                clientOrderId: clientOrderId,
                side: Bybit.Net.Enums.OrderSide.Buy,
                type: NewOrderType.Market,
                quantity: quantity);

            if (result.Success)
            {
                return PlaceOrderResult.Success(result.Data.OrderId);
            }            
            else
            {
                _logger.LogError(result.Error?.ToString());
                return PlaceOrderResult.Fail(result.Error?.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PlaceBuyOrder error.");
            return PlaceOrderResult.Fail(ex.Message);
        }
        finally
        {
            bybitClient?.Dispose();
        }
    }

    public async Task<PlaceOrderResult> PlaceSellOrder(string symbol, decimal quantity, string clientOrderId)
    {
        BybitRestClient? bybitClient = null;
        try
        {
            quantity = await _lotSizeFilterService.CorrectQuantity(symbol, quantity);

            bybitClient = CreateBybitClient();

            var result = await bybitClient.V5Api.Trading.PlaceOrderAsync(
                category: Category.Spot,
                symbol: symbol,
                clientOrderId: clientOrderId,
                side: Bybit.Net.Enums.OrderSide.Sell,
                type: NewOrderType.Market,
                quantity: quantity);

            return result.Success
                ? PlaceOrderResult.Success(result.Data.OrderId)
                : PlaceOrderResult.Fail(result.Error?.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PlaceBuyOrder error.");
            return PlaceOrderResult.Fail(ex.Message);
        }
        finally
        {
            bybitClient?.Dispose();
        }
    }

    public async Task<TradeOrder?> GetOrder(string orderId)
    {
        BybitRestClient? bybitClient = null;
        try
        {
            bybitClient = CreateBybitClient();

            var response = await bybitClient.V5Api.Trading.GetOrdersAsync(
                category: Category.Spot, 
                orderId: orderId);
            
            if (response.Success)
            {
                var order = response.Data.List.First();
                
                return TradeOrder.Create(
                    tradeOrderId: order.OrderId,
                    actualAveragePrice: order.AveragePrice,
                    quantity: order.Quantity,
                    quantityFilled: order.QuantityFilled,
                    executedFee: order.ExecutedFee,
                    valueFilled: order.ValueFilled,
                    valueRemaining: order.ValueRemaining,
                    status: BybitOrderStatusMapper.MapOrderStatus(order.Status),
                    orderSide: BybitOrderStatusMapper.MapOrderSide(order.Side)
                );
            }            
            else
            {
                _logger.LogError("cannot get order, id {orderId}. Reason: {reason}", orderId, response.Error?.ToString());
                return null;
            }
        }
        finally
        {
            bybitClient?.Dispose();
        }
    }

    private BybitRestClient CreateBybitClient()
    {
        var bybitClient = new BybitRestClient(
            httpClient: _httpClientFactory.CreateClient("bybit_client"),
            optionsDelegate: options =>
            {
                options.Environment = BybitEnvironment.Testnet;
                options.ApiCredentials = new ApiCredentials(_bybitSettings.Value.ApiKey, _bybitSettings.Value.ApiSecret);
            },
            loggerFactory: null);
        
        return bybitClient;
    }
}