using Bybit.Net;
using Bybit.Net.Clients;
using Bybit.Net.Enums;
using Bybit.Net.Objects.Models.V5;
using Chipseky.MamkinInvestor.Infrastructure.Options;
using Chipseky.MamkinInvestor.WebApi.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Chipseky.MamkinInvestor.WebApi.Controllers;

[ApiController]
public class DashboardController : ControllerBase
{
    [HttpGet("/api/dashboard/balance")]
    [ProducesResponseType<BybitBalance>(200)]
    public async Task<IActionResult> GetBalance(
        IHttpClientFactory httpClientFactory,
        IOptions<BybitSettings> bybitSettings,
        CancellationToken cancellationToken)
    {
        var bybitClient = new BybitRestClient(
            httpClient: httpClientFactory.CreateClient("bybit_client"),
            optionsDelegate: options =>
            {
                options.Environment = BybitEnvironment.Testnet;
                options.ApiCredentials = new ApiCredentials(bybitSettings.Value.ApiKey, bybitSettings.Value.ApiSecret);
            },
            loggerFactory: null);
        
        var result = await bybitClient.V5Api.Account.GetBalancesAsync(AccountType.Unified, null, cancellationToken);

        var utaAssets = result.Data.List.FirstOrDefault(a => a.AccountType == AccountType.Unified);

        return Ok(utaAssets);
    }
}