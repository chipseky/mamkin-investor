namespace Chipseky.MamkinInvestor.Domain;

public class OrdersManager
{
    private readonly IOrdersApi _ordersApi;
    
    private List<Order> _orders = new();

    public OrdersManager(IOrdersApi ordersApi)
    {
        _ordersApi = ordersApi;
    }

    public IEnumerable<Order> Orders => _orders;
    
    public async Task CreateByOrder(string tradingPair, int coinsAmount, decimal forecastedPrice)
    {
        var order = Order.CreateByOrder(tradingPair, coinsAmount, forecastedPrice);
        _orders.Add(order);
        await _ordersApi.PlaceBuyOrder(order.TradingPair, coinsAmount);
    }

    public async Task CreateSellOrder(string tradingPair, int coinsAmount, decimal forecastedPrice)
    {
        var order = Order.CreateSellOrder(tradingPair, coinsAmount, forecastedPrice);
        _orders.Add(order);
        await _ordersApi.PlaceSellOrder(order.TradingPair, coinsAmount);
    }
}