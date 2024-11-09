using Mamkin.In.Infrastructure;
using Mamkin.In.Infrastructure.Influx;
using Mamkin.In.WebApi.Contracts;
using Mamkin.In.WebApi.Contracts.Resp;
using Microsoft.AspNetCore.Mvc;

namespace Mamkin.In.WebApi.Controllers;

[ApiController]
public class TimeSeriesController : ControllerBase
{
    [HttpGet("/api/time-series")]
    [ProducesResponseType<IEnumerable<SymbolPrice>>(200)]
    public async Task<IActionResult> GetTimeSeries(
        [FromQuery] string symbol,
        [FromQuery] string range,
        [FromQuery] string interval,
        [FromServices] InfluxDbService dbService,
        CancellationToken cancellationToken)
    {
        var results = await dbService.Query(async query =>
        {
            var flux = $$"""
                    from(bucket: "bybit-bucket")
                        |> range(start: -{{range}})
                        |> filter(fn: (r) => r._measurement == "tickers" and r.ticker == "{{symbol}}")
                        |> aggregateWindow(every: {{interval}}, fn: mean, createEmpty: false)
                        |> sort(columns: ["_time"], desc: false)
                """;
            var tables = await query.QueryAsync(flux, "mamkin-investor", cancellationToken);
            return tables.SelectMany(table =>
                table.Records.Select(record =>
                    new SymbolPrice
                    {
                        Time = record.GetTimeInDateTime()!.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")!,
                        Price = Convert.ToDecimal(record.GetValue())
                    }));
        });

        return Ok(results);
    }
}