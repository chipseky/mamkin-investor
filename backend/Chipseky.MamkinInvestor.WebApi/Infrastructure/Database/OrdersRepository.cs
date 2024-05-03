using Chipseky.MamkinInvestor.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;

public class OrdersRepository : IOrdersRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrdersRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Order>> GetAll() => await _dbContext.Orders.AsNoTracking().ToListAsync();

    public async Task Save(Order order)
    {
        if (_dbContext.Entry(order).State == EntityState.Detached)
            _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();
    }
}