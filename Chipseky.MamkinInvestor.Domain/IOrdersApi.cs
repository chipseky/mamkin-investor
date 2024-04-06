namespace Chipseky.MamkinInvestor.Domain;

public interface IOrdersApi
{
    Task PlaceBuyOrder(string tradingPair, int coinsAmount);
    Task PlaceSellOrder(string tradingPair, int coinsAmount);
}