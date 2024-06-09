namespace Chipseky.MamkinInvestor.Domain;

public interface IOrdersApi
{
    Task<PlaceOrderResult> PlaceBuyOrder(string tradingPair, decimal coinsAmount);
    Task<PlaceOrderResult> PlaceSellOrder(string tradingPair, decimal coinsAmount);
}

public record PlaceOrderResult(bool Succeeded, decimal ActualPrice, decimal CoinsCount, string? ErrorReason = null)
{
    public static PlaceOrderResult Success(decimal actualPrice, decimal coinsCount) => new(true, coinsCount, actualPrice);
    public static PlaceOrderResult Fail(string errorReason) => new(false, -1, -1, errorReason);
}