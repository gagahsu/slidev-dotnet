using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace EShop.DataAccess.Repository;

public class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    private readonly ApplicationDbContext _db = db;

    public ICategoryRepository Category { get; } = new CategoryRepository(db);
    public IProductRepository Product { get; } = new ProductRepository(db);
    public IStoreRepository Store { get; } = new StoreRepository(db);
    public IApplicationUserRepository ApplicationUser { get; } = new ApplicationUserRepository(db);
    public IShoppingCartRepository ShoppingCart { get; } = new ShoppingCartRepository(db);
    public IOrderHeaderRepository OrderHeader { get; } = new OrderHeaderRepository(db);
    public IOrderDetailRepository OrderDetail { get; } = new OrderDetailRepository(db);

    public async Task SaveAsync() => await _db.SaveChangesAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync() => _db.Database.BeginTransactionAsync();
}
