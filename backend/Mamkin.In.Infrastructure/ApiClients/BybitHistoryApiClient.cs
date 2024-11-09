using Bybit.Net.Clients;
using Bybit.Net.Enums;
using CryptoExchange.Net.Authentication;
using Mamkin.In.Domain;
using Mamkin.In.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Mamkin.In.Infrastructure.ApiClients;

public class BybitHistoryApiClient : IHistoryApi
{
    private readonly IOptions<BybitSettings> _bybitSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    public BybitHistoryApiClient(
        IOptions<BybitSettings> bybitSettings, 
        IHttpClientFactory httpClientFactory)
    {
        _bybitSettings = bybitSettings;
        _httpClientFactory = httpClientFactory;
    }

    public async Task GetTicker()
    {
        var bybitClient = CreateBybitClient();
        var result = await bybitClient.V5Api.ExchangeData.GetTradeHistoryAsync(
            category: Category.Spot, 
            symbol:"BTCUSDT",
            
            limit: 1);
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