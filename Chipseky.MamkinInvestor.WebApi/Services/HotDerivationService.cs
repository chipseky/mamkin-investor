using Bybit.Net.Clients;
using Bybit.Net.Enums;
using Chipseky.MamkinInvestor.WebApi.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Options;

namespace Chipseky.MamkinInvestor.WebApi.Services;

public class HotDerivationService
{
    private readonly string _apiKey;
    private readonly string _apiSecret;

    public HotDerivationService(IOptions<BybitSettings> options)
    {
        _apiKey = options.Value.ApiKey;
        _apiSecret = options.Value.ApiSecret;
    }

    public async Task<string> GetAsString()
    {
        var bybitClient = new BybitRestClient(options =>
        {
            options.ApiCredentials = new ApiCredentials(_apiKey, _apiSecret);
        });
        
        var hotTickers = await bybitClient.DerivativesApi.ExchangeData.GetTickerAsync(Category.Undefined);
        
        var top10HotTickerStrings = hotTickers.Data
            .OrderByDescending(t => t.PriceChangePercentage24H)
            .Take(10)
            .Select(t => $"{t.Symbol} - {t.PriceChangePercentage24H * 100}% - {t.LastPrice}")
            .ToList();

        var result = string.Join('\n', top10HotTickerStrings);

        return result;
    }
}