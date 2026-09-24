using System.Linq.Expressions;

namespace EShop.DataAccess.Repository.IRepository;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeProperties = null);

    Task<T?> GetAsync(
        Expression<Func<T, bool>> filter,
        string? includeProperties = null,
        bool tracked = false);

    void Add(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
