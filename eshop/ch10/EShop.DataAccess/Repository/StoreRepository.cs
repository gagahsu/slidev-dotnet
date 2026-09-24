using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;

namespace EShop.DataAccess.Repository;

public class StoreRepository(ApplicationDbContext db) : Repository<Store>(db), IStoreRepository
{
    public void Update(Store store) => _db.Stores.Update(store);
}
