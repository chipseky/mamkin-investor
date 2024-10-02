using Chipseky.MamkinInvestor.Infrastructure.Options;
using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Options;

namespace Chipseky.MamkinInvestor.Infrastructure;

public class InfluxDbService : IDisposable
{
    private readonly InfluxDBClient _client;
    private readonly WriteApiAsync _writeApiAsync;
    private readonly string _organization;

    public InfluxDbService(IOptions<InfluxDbSettings> influxDbSettings)
    {
        _client = new InfluxDBClient(influxDbSettings.Value.Url, influxDbSettings.Value.Token);
        _writeApiAsync = _client.GetWriteApiAsync();
        _organization = influxDbSettings.Value.Organization;
    }
    
    public async Task<T> Query<T>(Func<QueryApi, Task<T>> action)
    {
        var query = _client.GetQueryApi();
        return await action(query);
    }
    
    public async Task Write(string bucket, PointData point)
    {
        await _writeApiAsync.WritePointAsync(point, bucket, _organization);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}