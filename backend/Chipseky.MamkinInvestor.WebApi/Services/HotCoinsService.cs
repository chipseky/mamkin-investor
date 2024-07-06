using Bybit.Net.Clients;
using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.Infrastructure.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Options;

namespace Chipseky.MamkinInvestor.WebApi.Services;

public class HotCoinsService
{
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly IHttpClientFactory _httpClientFactory;

    public HotCoinsService(IOptions<BybitSettings> options, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = options.Value.ApiKey;
        _apiSecret = options.Value.ApiSecret;
    }

    public async Task<Dictionary<string, SymbolPriceChange>> GetTop10TradingPairs()
    {
        var bybitClient = new BybitRestClient(
            httpClient: _httpClientFactory.CreateClient("bybit_client"),
            optionsDelegate: options =>
            {
                // options.Environment = BybitEnvironment.Testnet;
                options.ApiCredentials = new ApiCredentials(_apiKey, _apiSecret);
            },
            loggerFactory: null);

        var hotCoins = await bybitClient.V5Api.ExchangeData.GetSpotTickersAsync();
        
        var top10HotTicker = hotCoins.Data.List
            .OrderByDescending(t => t.PriceChangePercentag24h)
            .Take(10)
            .ToDictionary(
                t => t.Symbol,
                t => new SymbolPriceChange(
                    PriceChangePercentage24H: t.PriceChangePercentag24h,
                    LastPrice: t.LastPrice));

        return top10HotTicker;
    }
}