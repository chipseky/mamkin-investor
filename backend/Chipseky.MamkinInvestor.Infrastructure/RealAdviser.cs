using Chipseky.MamkinInvestor.Domain;

namespace Chipseky.MamkinInvestor.Infrastructure;

public class RealAdviser : IRealAdviser
{
    public async Task<bool> ShouldBuy(string symbol)
    {
        var randomValue = new Random().Next(100);
        return randomValue > 50;
    }

    public async Task<bool> ShouldSell(string symbol)
    {
        var randomValue = new Random().Next(100);
        return randomValue > 50;
    }
}