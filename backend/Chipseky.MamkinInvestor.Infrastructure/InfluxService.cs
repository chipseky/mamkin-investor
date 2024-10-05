using InfluxDB.Client;
using InfluxDB.Client.Writes;

namespace Chipseky.MamkinInvestor.Infrastructure;

public class InfluxService : IDisposable
{
    private readonly InfluxDBClient _client;
    private readonly WriteApiAsync _writeApiAsync;

    public InfluxService(string url, string token)
    {
        _client = new InfluxDBClient(url, token);
        _writeApiAsync = _client.GetWriteApiAsync();
    }
    
    public async Task<T> Query<T>(Func<QueryApi, Task<T>> action)
    {
        var query = _client.GetQueryApi();
        return await action(query);
    }
    
    public async Task Write(string bucket, PointData point)
    {
        await _writeApiAsync.WritePointAsync(point, bucket, "mamkin-investor");
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}

public class SymbolPrice
{
    public string Time { get; init; }
    public decimal Price { get; init; }
}