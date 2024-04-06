using Chipseky.MamkinInvestor.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Chipseky.MamkinInvestor.WebApi.Controllers;

[ApiController]
public class OrdersController : ControllerBase
{
    [HttpGet("/api/orders")]
    [ProducesResponseType<IEnumerable<Order>>(200)]
    public IActionResult GetOrders(
        [FromServices] OrdersManager ordersManager)
    {
        return Ok(ordersManager.Orders);
    }

    [HttpGet("/api/orders/profit")]
    [ProducesResponseType<string>(200)]
    public IActionResult CalcProfit(
        [FromServices] OrdersManager ordersManager)
    {
        var ordersSnapshot = ordersManager.Orders
            .Select(o => new
            {
                o.TradingPair,
                o.OrderType,
                Price = o.OrderType == OrderType.Buy ? -o.ForecastedPrice : o.ForecastedPrice
            })
            .ToList();

        var profit = ordersSnapshot
            .GroupBy(o => new { o.TradingPair })
            .Where(g => g.Count() % 2 == 0) // don't consider open positions
            .SelectMany(g => g.Select(i => i.Price))
            .Sum();

        return Ok($"your profit is approximately {profit}$");
    }
}