namespace Mamkin.In.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;

public record KeyDeleteDataChangeEvent : DataChangeEvent
{
    /// <summary>
    /// Gets or sets the keys having been deleted.
    /// </summary>
    public required IDictionary<string, object?> Keys { get; set; }
}