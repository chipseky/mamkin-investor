using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Dtos;

namespace Chipseky.MamkinInvestor.WebApi.Queries;

public class TradesTableDataQuery : IPagination
{
     public string TradingPair { get; set; }
     public TradeState? TradeState { get; set; }
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
     public TradeState State { get; set; }
     public IEnumerable<TradeOrder> Orders { get; set; }
}

public class TradeTableOrder
{
     public DateTime CreatedAt { get; set; }
     public decimal CoinsCount { get; set; }
     public decimal ActualPrice { get; set; }
     public OrderSide OrderSide { get; set; }
}