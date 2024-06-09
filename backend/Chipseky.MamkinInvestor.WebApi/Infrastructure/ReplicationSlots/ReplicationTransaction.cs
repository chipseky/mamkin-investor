using Chipseky.MamkinInvestor.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.ReplicationSlots;

/// <summary>
/// A Transaction sent by Postgres with all related DataChange Events.
/// </summary>
public record ReplicationTransaction
{
    public List<DataChangeEvent> ReplicationDataEvents { get; } = [];
}