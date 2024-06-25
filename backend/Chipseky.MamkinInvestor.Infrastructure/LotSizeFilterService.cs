using Bybit.Net;
using Bybit.Net.Clients;
using Bybit.Net.Objects.Models.V5;
using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.Infrastructure.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chipseky.MamkinInvestor.Infrastructure;

public class LotSizeFilterService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<BybitSettings> _bybitSettings;
    private readonly ILogger<LotSizeFilterService> _logger;
    private readonly IMemoryCache _memoryCache;

    public LotSizeFilterService(
        IHttpClientFactory httpClientFactory, 
        IOptions<BybitSettings> bybitSettings, 
        ILogger<LotSizeFilterService> logger, 
        IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _bybitSettings = bybitSettings;
        _logger = logger;
        _memoryCache = memoryCache;
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
        var lotSizeFilter = _memoryCache.Get<BybitSpotLotSizeFilter>(symbol);

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

            _memoryCache.Set(symbol, lotSizeFilter, DateTimeOffset.UtcNow.AddHours(1));

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
                options.Environment = BybitEnvironment.Testnet;
                options.ApiCredentials = new ApiCredentials(_bybitSettings.Value.ApiKey, _bybitSettings.Value.ApiSecret);
            },
            loggerFactory: null);
        
        return bybitClient;
    }
}