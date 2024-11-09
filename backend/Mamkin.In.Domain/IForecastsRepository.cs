namespace Mamkin.In.Domain;

public interface IForecastsRepository
{
    Task Store(Forecast forecast);
}