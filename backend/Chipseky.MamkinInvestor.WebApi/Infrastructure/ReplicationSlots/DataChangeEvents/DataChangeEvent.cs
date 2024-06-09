namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;

public abstract record DataChangeEvent
{
    public required Relation Relation { get; set; }
}