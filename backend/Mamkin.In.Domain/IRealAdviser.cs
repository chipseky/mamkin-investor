namespace Mamkin.In.Domain;

public interface IRealAdviser
{
    Task<(bool shouleBy, Forecast forecast)> ShouldBuy(string symbol);
    Task<bool> ShouldSell(string symbol);
}