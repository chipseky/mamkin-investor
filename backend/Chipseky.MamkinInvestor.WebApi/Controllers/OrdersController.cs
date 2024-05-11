using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Chipseky.MamkinInvestor.WebApi.Controllers;

[ApiController]
public class OrdersController : ControllerBase
{
    [HttpPost("/api/orders")]
    [ProducesResponseType<IPagedData<Order>>(200)]
    public async Task<IActionResult> GetOrders(
        [FromBody] OrdersTableDataQuery query, 
        [FromServices] OrdersTableDataQueryHandler ordersTableDataQueryHandler)
    {
        var orders = await ordersTableDataQueryHandler.Handle(query);
        
        return Ok(orders);
    }

    [HttpGet("/api/orders/profit")]
    [ProducesResponseType<string>(200)]
    public async Task<IActionResult> CalcProfit([FromServices] OrdersManager ordersManager)
    {
        return Ok($"your profit is approximately {await ordersManager.CalcProfit()}$");
    }
}