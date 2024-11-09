namespace Mamkin.In.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;

/// <summary>
/// A full update event contains the old and the new values.
/// </summary>
public record FullDeleteDataChangeEvent : DataChangeEvent
{
    /// <summary>
    /// Old column values.
    /// </summary>
    public required IDictionary<string, object?> OldValues { get; set; }
}