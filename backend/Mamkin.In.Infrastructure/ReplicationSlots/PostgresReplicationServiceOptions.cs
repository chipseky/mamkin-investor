namespace Mamkin.In.WebApi.Infrastructure.ReplicationSlots;

public class PostgresReplicationServiceOptions
{
    public required string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the PublicationName the Service is listening to.
    /// </summary>
    public required string PublicationName { get; set; }

    /// <summary>
    /// Gets or sets the ReplicationSlot the Service is listening to.
    /// </summary>
    public required string ReplicationSlotName { get; set; }
}