using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Chipseky.MamkinInvestor.WebApi.Controllers;

[ApiController]
public class SystemController : ControllerBase
{
    [HttpGet("/api/test")]
    public async Task<IActionResult> Test(BybitOrdersApi ordersApi)
    {
        var buyOrderRequest = await ordersApi.PlaceBuyOrder("BTCUSDT", 20m, Guid.NewGuid().ToString());
        var buyOrder = await ordersApi.GetOrder(buyOrderRequest.OrderId!);
        
        var sellOrderRequest = await ordersApi.PlaceSellOrder("BTCUSDT", 0.00016227m, Guid.NewGuid().ToString());
        var sellOrder = await ordersApi.GetOrder(sellOrderRequest.OrderId!);
        return Ok();
    }
    
    [HttpGet("/api/ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}