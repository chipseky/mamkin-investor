using Mamkin.In.Domain;
using Mamkin.In.Infrastructure.Ef;
using Microsoft.EntityFrameworkCore;

namespace Mamkin.In.Infrastructure.DbRepositories;

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