using Mamkin.In.Domain;
using Mamkin.In.Infrastructure;
using Mamkin.In.Infrastructure.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace Mamkin.In.WebApi.Controllers;

[ApiController]
public class SystemController : ControllerBase
{
    [HttpGet("/api/test")]
    public async Task<IActionResult> Test(BybitOrdersApiClient ordersApiClient)
    {
        var buyOrderRequest = await ordersApiClient.PlaceBuyOrder("BTCUSDT", 20m, Guid.NewGuid().ToString());
        var buyOrder = await ordersApiClient.GetOrder(buyOrderRequest.OrderId!);
        
        var sellOrderRequest = await ordersApiClient.PlaceSellOrder("BTCUSDT", 0.00016227m, Guid.NewGuid().ToString());
        var sellOrder = await ordersApiClient.GetOrder(sellOrderRequest.OrderId!);
        return Ok();
    }
    
    [HttpGet("/api/tickers")]
    public async Task<IActionResult> GetTickers(BybitHistoryApiClient historyApiClient)
    {
        await historyApiClient.GetTicker();
        
        return Ok();
    }
    
    [HttpGet("/api/ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}