namespace Mamkin.In.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;

public record DefaultUpdateDataChangeEvent : DataChangeEvent
{
    public required IDictionary<string, object?> NewValues { get; set; }
}