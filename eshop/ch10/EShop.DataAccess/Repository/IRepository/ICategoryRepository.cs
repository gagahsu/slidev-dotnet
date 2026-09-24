using EShop.Models;

namespace EShop.DataAccess.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
    Task<bool> IsNameExistsAsync(string name, int excludeId = 0);
}
