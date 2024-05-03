using Bybit.Net.Clients;
using Chipseky.MamkinInvestor.WebApi.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Chipseky.MamkinInvestor.WebApi.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("/api/test")]
    public async Task<IActionResult> Test(IOptions<BybitSettings> bybitSettings)
    {
        var bybitClient = new BybitRestClient(options =>
        {
            options.ApiCredentials = new ApiCredentials(bybitSettings.Value.ApiKey, bybitSettings.Value.ApiSecret);
        });
        
        // var tt = await bybitClient.V5Api.Trading.PlaceOrderAsync(
        //     category: Category.Spot,
        //     symbol: "BTCUSD",
        //     side: OrderSide.Buy,
        //     type: NewOrderType.Market,
        //     quantity: 1);

        return Ok();
    }
    
    [HttpGet("/api/ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}