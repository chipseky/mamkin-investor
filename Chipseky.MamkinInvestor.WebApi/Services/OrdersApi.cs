using Chipseky.MamkinInvestor.Domain;

namespace Chipseky.MamkinInvestor.WebApi.Services;

public class OrdersApi : IOrdersApi
{
    public async Task PlaceBuyOrder(string tradingPair, int coinsAmount)
    {
        // we dont' send anything to bybit:)
    }

    public async Task PlaceSellOrder(string tradingPair, int coinsAmount)
    {
        // we dont' send anything to bybit:)
    }
}