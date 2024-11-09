using Bybit.Net.Clients;
using Bybit.Net.Objects.Models.V5;
using Mamkin.In.Domain;
using Mamkin.In.Infrastructure.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamkin.In.Infrastructure;

public class LotSizeFilterService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<BybitSettings> _bybitSettings;
    private readonly ILogger<LotSizeFilterService> _logger;
    private readonly IDistributedCache _cache;

    public LotSizeFilterService(
        IHttpClientFactory httpClientFactory, 
        IOptions<BybitSettings> bybitSettings, 
        ILogger<LotSizeFilterService> logger, 
        IDistributedCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _bybitSettings = bybitSettings;
        _logger = logger;
        _cache = cache;
    }

    public async Task<decimal> CorrectQuantity(string symbol, decimal quantity)
    {
        var lotSizeFilter = await GetLotSizeFilter(symbol);

        var quantityDecimals = LotSizeFilterHelper.GetDecimals(lotSizeFilter.BasePrecision);
        
        if(quantity > 0)
            quantity = Math.Round(quantity, quantityDecimals, MidpointRounding.ToZero);
        
        return quantity;
    }

    private async Task<BybitSpotLotSizeFilter> GetLotSizeFilter(string symbol)
    {
        var lotSizeFilter = await _cache.GetValue<BybitSpotLotSizeFilter>(symbol, CancellationToken.None);
        
        if (lotSizeFilter != null)
            return lotSizeFilter;

        BybitRestClient? bybitClient = null;
        try
        {
            bybitClient = CreateBybitClient();

            var response = await bybitClient.V5Api.ExchangeData.GetSpotSymbolsAsync(symbol);

            if (response.Success)
            {
                lotSizeFilter = response.Data.List.First().LotSizeFilter!;
            }
            else
            {
                _logger.LogError(response.Error?.Message);
                throw new InvalidOperationException($"Cannot read instruments info. Symbol is {symbol}");
            }
            
            await _cache.SetValue(
                key: symbol, 
                value: lotSizeFilter, 
                options: new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }, 
                cancellationToken: CancellationToken.None);
        }
        finally
        {
            bybitClient?.Dispose();
        }
        
        return lotSizeFilter;
    }
    
    private BybitRestClient CreateBybitClient()
    {
        var bybitClient = new BybitRestClient(
            httpClient: _httpClientFactory.CreateClient("bybit_client"),
            optionsDelegate: options =>
            {
                // options.Environment = BybitEnvironment.Testnet;
                options.ApiCredentials = new ApiCredentials(_bybitSettings.Value.ApiKey, _bybitSettings.Value.ApiSecret);
            },
            loggerFactory: null);
        
        return bybitClient;
    }
}