namespace Chipseky.MamkinInvestor.Domain;

public interface IOrdersRepository
{
    Task<IEnumerable<Order>> GetAll();
    Task Save(Order order);
}