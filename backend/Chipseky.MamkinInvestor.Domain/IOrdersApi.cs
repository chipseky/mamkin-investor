namespace Chipseky.MamkinInvestor.Domain;

public interface IOrdersApi
{
    Task<PlaceOrderResult> PlaceBuyOrder(string symbol, decimal quantity, string clientOrderId);
    Task<PlaceOrderResult> PlaceSellOrder(string symbol, decimal quantity, string clientOrderId);
    Task<TradeOrder> GetOrder(string orderId);
}

public record PlaceOrderResult(bool Succeeded, string? OrderId, string? Error = null)
{
    public static PlaceOrderResult Success(string orderId) => new(true, orderId);
    public static PlaceOrderResult Fail(string? error) => new(false, null,  error);
}