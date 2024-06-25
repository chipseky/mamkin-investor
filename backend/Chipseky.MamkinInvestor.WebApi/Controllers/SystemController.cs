using Chipseky.MamkinInvestor.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Chipseky.MamkinInvestor.WebApi.Controllers;

[ApiController]
public class SystemController : ControllerBase
{
    [HttpGet("/api/test")]
    public async Task<IActionResult> Test(IOrdersApi ordersApi)
    {
        // await ordersApi.PlaceSellOrder("BTCUSDT", 0.00622377m, Guid.NewGuid().ToString());
        var order = await ordersApi.GetOrder("1719081799784950016");
        // var result = await ordersApi.PlaceSellOrder("BTCUSDT", 0.0001676m, Guid.NewGuid().ToString());
        return Ok();
    }
    
    [HttpGet("/api/ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}