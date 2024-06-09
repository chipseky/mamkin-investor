using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using NpgsqlTypes;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.ReplicationSlots;

/// <summary>
/// The Service processes Replication Events published by a Postgres publication.
/// </summary>
public class PostgresReplicationService
{
    private readonly ILogger<PostgresReplicationService> _logger;
    private readonly TradeEventsHandler _tradeEventsHandler;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// Options to configure the Wal Receiver.
    /// </summary>
    private readonly PostgresReplicationServiceOptions _options;

    public PostgresReplicationService(ILogger<PostgresReplicationService> logger,
        IOptions<PostgresReplicationServiceOptions> options, TradeEventsHandler tradeEventsHandler, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _tradeEventsHandler = tradeEventsHandler;
        _serviceScopeFactory = serviceScopeFactory;
        _options = options.Value;
    }

    /// <summary>
    /// Instructs the server to start the Logical Streaming Replication Protocol (pgoutput logical decoding 
    /// plugin), starting at WAL location walLocation or at the slot's consistent point if walLocation isn't 
    /// specified. The server can reply with an error, for example if the requested section of the WAL has 
    /// already been recycled.
    /// </summary>
    /// <returns>Replication Transactions</returns>
    /// <exception cref="InvalidOperationException">Thrown when a replication message can't be handled</exception>
    public async IAsyncEnumerable<ReplicationTransaction> StartReplicationAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var lastConfirmedLsn = await LoadLsn();
        // This is the only way to create the Replication Connection, I have found no 
        // way to utilize the NpgsqlDataSource for it. There might be a way though.
        await using var replicationConnection = new LogicalReplicationConnection(_options.ConnectionString);

        await replicationConnection
            .Open(cancellationToken);

        // Reference to the Publication.
        var replicationPublication =
            new PgOutputReplicationOptions(_options.PublicationName, protocolVersion: 1, binary: true);

        // Reference to the Replication Slot.
        var replicationSlot = new PgOutputReplicationSlot(_options.ReplicationSlotName);

        // Postgres expects us to cache all relations.
        var relations = new ConcurrentDictionary<uint, Relation>();

        // The current transaction, which will be set, when we receive the first commit message.
        ReplicationTransaction transaction = null!;

        await foreach (var message in replicationConnection
                           .StartReplication(replicationSlot, replicationPublication, cancellationToken, lastConfirmedLsn))
        {
            _logger.LogDebug(
                "Received Postgres WAL Message (Type = {WalMessageType}, ServerClock = {WalServerClock}, WalStart = {WalStart}, WalEnd = {WalEnd})",
                message.GetType().Name, message.ServerClock, message.WalStart, message.WalEnd);

            switch (message)
            {
                case BeginMessage:
                    transaction = new ReplicationTransaction();
                    break;
                case CommitMessage:
                    yield return transaction;
                    break;
                case RelationMessage relationMessage:
                    relations[relationMessage.RelationId] = new Relation
                    {
                        RelationId = relationMessage.RelationId,
                        Namespace = relationMessage.Namespace,
                        RelationName = relationMessage.RelationName,
                        ServerClock = relationMessage.ServerClock,
                        ColumnNames = relationMessage.Columns
                            .Select(x => x.ColumnName)
                            .ToArray()
                    };
                    break;
                case InsertMessage insertMessage:
                {
                    var relation = relations[insertMessage.Relation.RelationId];

                    var insertDataChangeEvent = new InsertDataChangeEvent
                    {
                        Relation = relation,
                        NewValues = await ReadColumnValuesAsync(relation, insertMessage.NewRow, cancellationToken)
                    };
                    
                    transaction.ReplicationDataEvents.Add(insertDataChangeEvent);

                    if (relation.RelationName == "trade_events")
                        await _tradeEventsHandler.Handle(TradeEventsParser.ParseEventData(insertDataChangeEvent));
                    
                    break;
                }
                case DefaultUpdateMessage defaultUpdateMessage:
                {
                    var relation = relations[defaultUpdateMessage.Relation.RelationId];

                    transaction.ReplicationDataEvents.Add(new DefaultUpdateDataChangeEvent
                    {
                        Relation = relation,
                        NewValues = await ReadColumnValuesAsync(relation, defaultUpdateMessage.NewRow, cancellationToken)
                    });
                    break;
                }
                case FullUpdateMessage fullUpdateMessage:
                {
                    var relation = relations[fullUpdateMessage.Relation.RelationId];

                    transaction.ReplicationDataEvents.Add(new FullUpdateDataChangeEvent
                    {
                        Relation = relation,
                        NewValues = await ReadColumnValuesAsync(relation, fullUpdateMessage.NewRow, cancellationToken),
                        OldValues = await ReadColumnValuesAsync(relation, fullUpdateMessage.OldRow, cancellationToken)
                    });
                    break;
                }
                case KeyDeleteMessage keyDeleteMessage:
                {
                    var relation = relations[keyDeleteMessage.Relation.RelationId];

                    transaction.ReplicationDataEvents.Add(new KeyDeleteDataChangeEvent
                    {
                        Relation = relation,
                        Keys = await ReadColumnValuesAsync(relation, keyDeleteMessage.Key, cancellationToken)
                    });
                    break;
                }
                case FullDeleteMessage fullDeleteMessage:
                {
                    var relation = relations[fullDeleteMessage.Relation.RelationId];
                
                    transaction.ReplicationDataEvents.Add(new FullDeleteDataChangeEvent
                    {
                        Relation = relation,
                        OldValues = await ReadColumnValuesAsync(relation, fullDeleteMessage.OldRow, cancellationToken)
                    });
                    break;
                }
                default:
                    // We don't know what to do here and everything we could do... feels wrong. Throw 
                    // up to the consumer and let them handle the problem.
                    throw new InvalidOperationException($"Could not handle Message Type {message.GetType().Name}");
            }

            // Acknowledge the message.
            replicationConnection.SetReplicationStatus(message.WalEnd);
        }
    }

    private async Task<NpgsqlLogSequenceNumber> LoadLsn()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var lastConfirmedLsn = await dbContext.Database
            // I use as "Value" because of this: https://github.com/dotnet/efcore/issues/30447#issuecomment-1462231912
            .SqlQuery<NpgsqlLogSequenceNumber>(
                $"select confirmed_flush_lsn as \"Value\" from pg_replication_slots where slot_name = 'mamkin_investor_trade_events_slot'")
            .FirstAsync();

        return lastConfirmedLsn;
    }

    private async ValueTask<IDictionary<string, object?>> ReadColumnValuesAsync(Relation relation,
        ReplicationTuple replicationTuple, CancellationToken cancellationToken)
    {
        var results = new ConcurrentDictionary<string, object?>();

        // We need to track the current Column:
        int columnIdx = 0;

        // Each "ReplicationTuple" consists of multiple "ReplicationValues", that we could iterate over.
        await foreach (var replicationValue in replicationTuple)
        {
            // These "ReplicationValues" do not carry the column name, so we resolve the column name
            // from the associated relation. This is going to throw, if we cannot find the column name, 
            // but it should throw... because it is exceptional.
            var column = relation.ColumnNames[columnIdx];

            // Get the column value and let Npgsql decide, how to map the value. You could register
            // type mappers for the LogicalReplicationConnection, so you can also automagically map 
            // unknown types.
            //
            // This is going to throw, if Npgsql fails to read the values.
            var value = await replicationValue.Get(cancellationToken);

            // If we fail to add the value to the Results, there is not much we can do. Log it 
            // and go ahead.
            if (!results.TryAdd(column, value))
            {
                _logger.LogInformation("Failed to map ReplicationValue for Column {ColumnName}", column);
            }

            // Process next column
            columnIdx++;
        }

        return results;
    }
}