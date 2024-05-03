using Bybit.Net.Clients;
using Bybit.Net.Enums;
using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Options;

namespace Chipseky.MamkinInvestor.WebApi.Services;

public class HotDerivativesService
{
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly IHttpClientFactory _httpClientFactory;

    public HotDerivativesService(IOptions<BybitSettings> options, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = options.Value.ApiKey;
        _apiSecret = options.Value.ApiSecret;
    }

    public async Task<Dictionary<string, TradingPairPriceChange>> GetTop10TradingPairs()
    {
        var bybitClient = new BybitRestClient(
            httpClient: _httpClientFactory.CreateClient(),
            optionsDelegate: o => { o.ApiCredentials = new ApiCredentials(_apiKey, _apiSecret); },
            loggerFactory: null);

        var hotTickers = await bybitClient.DerivativesApi.ExchangeData.GetTickerAsync(Category.Undefined);

        var top10HotTicker = hotTickers.Data
            .OrderByDescending(t => t.PriceChangePercentage24H)
            .Take(10)
            .ToDictionary(
                t => t.Symbol,
                t => new TradingPairPriceChange(
                    PriceChangePercentage24H: t.PriceChangePercentage24H ?? 0,
                    LastPrice: t.LastPrice));

        return top10HotTicker;
    }
}