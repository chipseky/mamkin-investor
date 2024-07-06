namespace Chipseky.MamkinInvestor.Domain;

public interface IRealAdviser
{
    Task<bool> ShouldBuy(string symbol);
    Task<bool> ShouldSell(string symbol);
}