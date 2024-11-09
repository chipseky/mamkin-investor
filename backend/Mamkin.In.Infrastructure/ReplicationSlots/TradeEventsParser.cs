using System.Text.Json;
using Mamkin.In.Infrastructure.Ef;
using Mamkin.In.Infrastructure.Extensions;
using Mamkin.In.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;

namespace Mamkin.In.Infrastructure.ReplicationSlots;

public class TradeEventsParser
{
    public static object ParseEventData(InsertDataChangeEvent tradeEventRecord)
    {
        var eventType = GetEventType(tradeEventRecord);
        var eventData = GetEventData(tradeEventRecord, eventType);
        return eventData;
    }
    
    private static string GetEventType(InsertDataChangeEvent tradeEventRecord)
    {
        var eventTypeFieldName = nameof(DbTradeEvent.Type).ToSnakeCase();
        var eventType = (string)tradeEventRecord.NewValues[eventTypeFieldName]!;
        return eventType;
    }

    private static object GetEventData(InsertDataChangeEvent tradeEventRecord, string typeName)
    {
        var dataFieldName = nameof(DbTradeEvent.Data).ToSnakeCase();
        var dataJson = (string)tradeEventRecord.NewValues[dataFieldName]!;
        var type = Type.GetType(typeName);
        return JsonSerializer.Deserialize(dataJson, type!)!;
    }
}