namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.ReplicationSlots;

/// <summary>
/// Postgres send a Relation Message during the Logical Replication.
/// </summary>
public record Relation
{
    public required uint RelationId { get; set; }

    public required string? Namespace { get; set; }

    public required string RelationName { get; set; }

    public required DateTime ServerClock { get; set; }

    public required string[] ColumnNames { get; set; }
}