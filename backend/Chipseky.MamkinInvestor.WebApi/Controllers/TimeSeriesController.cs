using Chipseky.MamkinInvestor.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Chipseky.MamkinInvestor.WebApi.Controllers;

[ApiController]
public class TimeSeriesController : ControllerBase
{
    [HttpGet("/api/time-series")]
    // [ProducesResponseType<BybitBalance>(200)]
    public async Task<IActionResult> GetTimeSeries(
        [FromQuery] string symbol,
        [FromServices] InfluxService service,
        CancellationToken cancellationToken)
    {
        var results = await service.Query(async query =>
        {
            var flux = """
                    from(bucket: "bybit-bucket")
                        |> range(start: -2h)
                        |> filter(fn: (r) => r._measurement == "tickers" and r.ticker == "BTCUSDT")
                        |> aggregateWindow(every: 1s, fn: mean, createEmpty: false)
                        |> sort(columns: ["_time"], desc: false)
                """;
            var tables = await query.QueryAsync(flux, "mamkin-investor", cancellationToken);
            return tables.SelectMany(table =>
                table.Records.Select(record =>
                    new SymbolPrice
                    {
                        Time = record.GetTimeInDateTime().ToString()!,
                        Price = Convert.ToDecimal(record.GetValue())
                    }));
        });

        return Ok(results);
    }
}