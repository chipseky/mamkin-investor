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

    public async Task Write(PointData point)
    {
        await _writeApiAsync.WritePointAsync(point, "test-bucket", "0e94d4a5b0f1f565");
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}