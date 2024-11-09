namespace Mamkin.In.Domain;

public interface IForecastApi
{
    Task<Forecast> Get(string symbol);
}