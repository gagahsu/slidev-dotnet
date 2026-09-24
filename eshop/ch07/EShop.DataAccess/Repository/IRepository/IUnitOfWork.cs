namespace EShop.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    Task SaveAsync();
}
