using Chipseky.MamkinInvestor.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Chipseky.MamkinInvestor.WebApi.Controllers;

[ApiController]
public class OrdersController : ControllerBase
{
    [HttpGet("/api/orders")]
    [ProducesResponseType<IEnumerable<Order>>(200)]
    public async Task<IActionResult> GetOrders([FromServices] OrdersManager ordersManager)
    {
        return Ok(await ordersManager.GetAllOrders());
    }

    [HttpGet("/api/orders/profit")]
    [ProducesResponseType<string>(200)]
    public async Task<IActionResult> CalcProfit([FromServices] OrdersManager ordersManager)
    {
        return Ok($"your profit is approximately {await ordersManager.CalcProfit()}$");
    }
}