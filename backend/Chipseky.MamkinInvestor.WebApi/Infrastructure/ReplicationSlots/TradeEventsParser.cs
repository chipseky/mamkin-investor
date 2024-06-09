using System.Text.Json;
using Chipseky.MamkinInvestor.WebApi.Extensions;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.ReplicationSlots.DataChangeEvents;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.ReplicationSlots;

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