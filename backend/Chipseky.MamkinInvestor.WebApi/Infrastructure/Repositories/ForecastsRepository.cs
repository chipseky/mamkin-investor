using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Repositories;

public class ForecastsRepository : IForecastsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ForecastsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Store(Forecast forecast)
    {
        _dbContext.Forecasts.Add(forecast);
        await _dbContext.SaveChangesAsync();
    }
}