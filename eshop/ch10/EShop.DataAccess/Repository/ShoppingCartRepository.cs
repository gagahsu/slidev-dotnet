using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;

namespace EShop.DataAccess.Repository;

public class ShoppingCartRepository(ApplicationDbContext db)
    : Repository<ShoppingCart>(db), IShoppingCartRepository
{
    public void Update(ShoppingCart cart) => _db.ShoppingCarts.Update(cart);
}
