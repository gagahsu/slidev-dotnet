using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.DataAccess.Repository;

public class CategoryRepository(ApplicationDbContext db)
    : Repository<Category>(db), ICategoryRepository
{
    public void Update(Category category) => _db.Categories.Update(category);

    public Task<bool> IsNameExistsAsync(string name, int excludeId = 0)
        => _db.Categories.AnyAsync(c => c.Name == name && c.Id != excludeId);
}
