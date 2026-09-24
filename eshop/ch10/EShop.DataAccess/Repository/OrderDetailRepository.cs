using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;

namespace EShop.DataAccess.Repository;

public class OrderDetailRepository(ApplicationDbContext db)
    : Repository<OrderDetail>(db), IOrderDetailRepository
{
    public void Update(OrderDetail detail) => _db.OrderDetails.Update(detail);
}
