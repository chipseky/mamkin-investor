using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Dtos;

namespace Chipseky.MamkinInvestor.WebApi.Queries;

public class TradesTableDataQuery : IPagination
{
     public string TradingPair { get; set; }
     public TradesTableOrderType OrdersType { get; set; }
     public int Page { get; set; }
     public int PageSize { get; set; }
}

public class TradesTableItem
{
     public Guid TradeId { get; set; }
     public string TradingPair { get; set; }
     public DateTime CreatedAt { get; set; }
     public decimal HeldCoinsCount { get; set; }
     public decimal CurrentProfit { get; set; }
     public bool Closed { get; set; }
     public IEnumerable<TradeOrder> Orders { get; set; }
}

public class TradeTableOrder
{
     public DateTime CreatedAt { get; set; }
     public decimal CoinsCount { get; set; }
     public decimal ActualPrice { get; set; }
     public OrderType OrderType { get; set; }
}

public enum TradesTableOrderType
{
    All,
    Opened,
    Closed
}