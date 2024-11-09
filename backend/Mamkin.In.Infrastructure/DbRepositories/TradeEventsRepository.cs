using Mamkin.In.Domain;
using Mamkin.In.Domain.TradeEvents;
using Mamkin.In.Infrastructure.Ef;

namespace Mamkin.In.Infrastructure.DbRepositories;

public class TradeEventsRepository : ITradeEventsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TradeEventsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Store(TradeEvent tradeEvent)
    {
        _dbContext.TradeEvents.Add(new DbTradeEvent
        {   
            Type = tradeEvent.GetType().AssemblyQualifiedName!,
            Data = tradeEvent
        });

        await _dbContext.SaveChangesAsync();
    }
}