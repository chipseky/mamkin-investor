namespace Chipseky.MamkinInvestor.Domain;

public interface IForecastApi
{
    Task<Forecast> Get(string symbol);
}