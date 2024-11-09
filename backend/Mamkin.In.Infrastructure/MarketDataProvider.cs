using Bybit.Net.Clients;
using Mamkin.In.Domain;
using Mamkin.In.Infrastructure.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Options;

namespace Mamkin.In.Infrastructure;

public class MarketDataProvider : IMarketDataProvider
{
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly IHttpClientFactory _httpClientFactory;

    public MarketDataProvider(IHttpClientFactory httpClientFactory, IOptions<BybitSettings> options)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = options.Value.ApiKey;
        _apiSecret = options.Value.ApiSecret;
    }

    public async Task<IEnumerable<MarketSymbol>> Get()
    {
        
        var bybitClient = new BybitRestClient(
            httpClient: _httpClientFactory.CreateClient("bybit_client"),
            optionsDelegate: options =>
            {
                // options.Environment = BybitEnvironment.Testnet;
                options.ApiCredentials = new ApiCredentials(_apiKey, _apiSecret);
            },
            loggerFactory: null);
        
        var allSymbols = await bybitClient.V5Api.ExchangeData.GetSpotTickersAsync();
        var result =
            allSymbols.Data.List.Select(s => new MarketSymbol(s.Symbol, s.LastPrice, s.PriceChangePercentag24h));
        
        return result;
    }
}