using Microsoft.EntityFrameworkCore.Storage;

namespace EShop.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    IStoreRepository Store { get; }
    IApplicationUserRepository ApplicationUser { get; }
    IShoppingCartRepository ShoppingCart { get; }
    IOrderHeaderRepository OrderHeader { get; }
    IOrderDetailRepository OrderDetail { get; }

    Task SaveAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();       // 扣庫存需要
}
