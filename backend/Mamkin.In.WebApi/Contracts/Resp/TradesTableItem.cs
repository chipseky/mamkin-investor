using Mamkin.In.Domain;

namespace Mamkin.In.WebApi.Contracts.Resp;

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