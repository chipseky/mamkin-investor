namespace Mamkin.In.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;

/// <summary>
/// An insert event includes the new values.
/// </summary>
public record InsertDataChangeEvent : DataChangeEvent
{
    /// <summary>
    /// New column values.
    /// </summary>
    public required IDictionary<string, object?> NewValues { get; set; }
}