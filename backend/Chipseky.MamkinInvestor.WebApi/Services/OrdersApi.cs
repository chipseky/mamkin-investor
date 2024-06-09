using Chipseky.MamkinInvestor.Domain;

namespace Chipseky.MamkinInvestor.WebApi.Services;

public class OrdersApi : IOrdersApi
{
    public async Task<PlaceOrderResult> PlaceBuyOrder(string tradingPair, decimal coinsAmount)
    {
        // we dont' send anything to bybit:)
        return PlaceOrderResult.Success(11, 1);
    }

    public async Task<PlaceOrderResult> PlaceSellOrder(string tradingPair, decimal coinsAmount)
    {
        // we dont' send anything to bybit:)
        return PlaceOrderResult.Success(12, 2);
    }
}