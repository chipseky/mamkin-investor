namespace Chipseky.MamkinInvestor.Domain;

public class OrdersManager
{
    private readonly IOrdersApi _ordersApi;
    private readonly IOrdersRepository _ordersRepository;
    
    public OrdersManager(IOrdersApi ordersApi, IOrdersRepository ordersRepository)
    {
        _ordersApi = ordersApi;
        _ordersRepository = ordersRepository;
    }

    public async Task<IEnumerable<Order>> GetAllOrders() => await _ordersRepository.GetAll();

    public async Task<decimal> CalcProfit()
    {
        var allOrders = await _ordersRepository.GetAll();

        var result = allOrders.Select(o => new
            {
                o.TradingPair,
                o.OrderType,
                Price = o.OrderType == OrderType.Buy ? -o.ForecastedPrice : o.ForecastedPrice
            })
            .GroupBy(o => new { o.TradingPair })
            .Where(g => g.Count() % 2 == 0) // don't consider open positions
            .SelectMany(g => g.Select(i => i.Price))
            .Sum();

        return result;
    }

    public async Task CreateByOrder(string tradingPair, int coinsAmount, decimal forecastedPrice)
    {
        var order = Order.CreateByOrder(tradingPair, coinsAmount, forecastedPrice);
        await _ordersApi.PlaceBuyOrder(order.TradingPair, coinsAmount);
        await _ordersRepository.Save(order);
    }

    public async Task CreateSellOrder(string tradingPair, int coinsAmount, decimal forecastedPrice)
    {
        var order = Order.CreateSellOrder(tradingPair, coinsAmount, forecastedPrice);
        await _ordersApi.PlaceSellOrder(order.TradingPair, coinsAmount);
        await _ordersRepository.Save(order);
    }
}