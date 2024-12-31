using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mamkin.In.Infrastructure.ReplicationSlots;

/// <summary>
/// This Listener waits for Data Change Events sent by Logical Replication.
/// </summary>
public class PostgresReplicationListener : BackgroundService
{
    private readonly ILogger<PostgresReplicationListener> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public PostgresReplicationListener(ILogger<PostgresReplicationListener> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        using var scope = _serviceScopeFactory.CreateScope();
        
        var replicationService = scope.ServiceProvider.GetRequiredService<PostgresReplicationService>();

        while (true)
        {
            try
            {
                await foreach (var transaction in replicationService.StartReplicationAsync(stoppingToken))
                {
                    _logger.LogDebug("Received Transaction: {Transaction}",
                        JsonSerializer.Serialize(transaction, jsonSerializerOptions));
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Replication slot sync error.");
            }

            await Task.Delay(1000, stoppingToken);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}