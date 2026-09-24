using EShop.Models;

namespace EShop.DataAccess.Repository.IRepository;

public interface IStoreRepository : IRepository<Store>
{
    void Update(Store store);
}
