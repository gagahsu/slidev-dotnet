using EShop.Models;

namespace EShop.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader order);
    Task UpdateStatusAsync(int id, string orderStatus);
}
