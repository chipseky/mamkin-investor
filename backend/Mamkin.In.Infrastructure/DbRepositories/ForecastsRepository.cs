using Mamkin.In.Domain;
using Mamkin.In.Infrastructure.Ef;

namespace Mamkin.In.Infrastructure.DbRepositories;

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