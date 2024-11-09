using Mamkin.In.Domain.Repositories;
using Mamkin.In.WebApi.Contracts.Paging;
using Mamkin.In.WebApi.Contracts.Req;
using Mamkin.In.WebApi.Contracts.Resp;
using Mamkin.In.WebApi.QueryHandlers;
using Microsoft.AspNetCore.Mvc;

namespace Mamkin.In.WebApi.Controllers;

[ApiController]
public class TradesController : ControllerBase
{
    [HttpPost("/api/trades")]
    [ProducesResponseType<IPagedData<TradesTableItem>>(200)]
    public async Task<IActionResult> GetTrades(
        [FromBody] TradesQuery query,
        [FromServices] TradesQueryHandler queryHandler)
    {
        var trades = await queryHandler.Handle(query);

        return Ok(trades);
    }
    
    [HttpPost("/api/trade-events")]
    [ProducesResponseType<IPagedData<object>>(200)]
    public async Task<IActionResult> GetTradeEvents(
        [FromBody] TradeEventsQuery query,
        [FromServices] TradeEventsQueryHandler queryHandler)
    {
        var tradeEvents = await queryHandler.Handle(query);

        return Ok(tradeEvents);
    }

    [HttpGet("/api/trades/profit")]
    [ProducesResponseType<string>(200)]
    public async Task<IActionResult> CalcProfit([FromServices] ITradesRepository tradesRepository, CancellationToken cancellationToken)
    {
        return Ok($"your profit is approximately {await tradesRepository.GetProfit(cancellationToken)}$");
    }
}