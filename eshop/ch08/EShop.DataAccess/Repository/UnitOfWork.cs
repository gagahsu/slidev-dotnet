using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;

namespace EShop.DataAccess.Repository;

public class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    private readonly ApplicationDbContext _db = db;

    public ICategoryRepository Category { get; } = new CategoryRepository(db);
    public IProductRepository Product { get; } = new ProductRepository(db);

    public async Task SaveAsync() => await _db.SaveChangesAsync();
}
