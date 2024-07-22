namespace Chipseky.MamkinInvestor.Domain;

public interface IForecastsRepository
{
    Task Store(Forecast forecast);
}