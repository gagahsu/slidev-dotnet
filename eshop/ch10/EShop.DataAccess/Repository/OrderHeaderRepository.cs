using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using EShop.Utility;
using Microsoft.EntityFrameworkCore;

namespace EShop.DataAccess.Repository;

public class OrderHeaderRepository(ApplicationDbContext db)
    : Repository<OrderHeader>(db), IOrderHeaderRepository
{
    public void Update(OrderHeader order) => _db.OrderHeaders.Update(order);

    public async Task UpdateStatusAsync(int id, string orderStatus)
    {
        var order = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == id);
        if (order is null) return;
        order.OrderStatus = orderStatus;
        if (orderStatus == SD.StatusShipped) order.ShippingDate = DateTime.Now;
    }
}
