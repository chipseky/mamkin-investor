using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Repositories;

public class PredefinedSymbolsRepository : IPredefinedSymbolsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PredefinedSymbolsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<PredefinedSymbol>> Get()
    {
        return await _dbContext.PredefinedSymbols
            .ToListAsync();
    }

    public async Task<TimeSpan?> GetForecastedSellOffset(string symbol)
    {
        var predefinedSymbol = await _dbContext.PredefinedSymbols.FirstOrDefaultAsync(ps => ps.Symbol == symbol);
        return predefinedSymbol?.ForecastedSellOffset;
    }
}