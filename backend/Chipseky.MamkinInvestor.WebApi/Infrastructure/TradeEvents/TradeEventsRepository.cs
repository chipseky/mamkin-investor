using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.Domain.TradeEvents;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.TradeEvents;

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