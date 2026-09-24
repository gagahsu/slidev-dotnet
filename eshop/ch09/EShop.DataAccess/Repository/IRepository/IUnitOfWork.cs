namespace EShop.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    IStoreRepository Store { get; }
    Task SaveAsync();
}
