using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Queries;
using Chipseky.MamkinInvestor.WebApi.QueryHandlers;
using Microsoft.AspNetCore.Mvc;

namespace Chipseky.MamkinInvestor.WebApi.Controllers;

[ApiController]
public class TradesController : ControllerBase
{
    [HttpPost("/api/trades")]
    [ProducesResponseType<IPagedData<TradesTableItem>>(200)]
    public async Task<IActionResult> GetTrades(
        [FromBody] TradesTableDataQuery query,
        [FromServices] TradesTableDataQueryHandler tradesTableDataQueryHandler)
    {
        var trades = await tradesTableDataQueryHandler.Handle(query);

        return Ok(trades);
    }
    
    [HttpPost("/api/trade-events")]
    [ProducesResponseType<IPagedData<object>>(200)]
    public async Task<IActionResult> GetTradeEvents(
        [FromBody] TradeEventsTableDataQuery query,
        [FromServices] TradeEventsTableDataQueryHandler tradeEventsTableDataQueryHandler)
    {
        var trades = await tradeEventsTableDataQueryHandler.Handle(query);

        return Ok(trades);
    }

    [HttpGet("/api/trades/profit")]
    [ProducesResponseType<string>(200)]
    public async Task<IActionResult> CalcProfit([FromServices] OrdersManager ordersManager)
    {
        return Ok($"your profit is approximately {await ordersManager.CalcProfit()}$");
    }
}