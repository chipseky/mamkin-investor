using Mamkin.In.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;

namespace Mamkin.In.WebApi.Infrastructure.ReplicationSlots;

/// <summary>
/// A Transaction sent by Postgres with all related DataChange Events.
/// </summary>
public record ReplicationTransaction
{
    public List<DataChangeEvent> ReplicationDataEvents { get; } = [];
}