using System.Linq.Expressions;
using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EShop.DataAccess.Repository;

public class Repository<T>(ApplicationDbContext db) : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _db = db;
    private readonly DbSet<T> _dbSet = db.Set<T>();

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        if (filter is not null) query = query.Where(filter);
        query = Include(query, includeProperties);
        if (orderBy is not null) query = orderBy(query);
        return await query.ToListAsync();          // 條件全部串完，最後才執行
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> filter,
        string? includeProperties = null, bool tracked = false)
    {
        IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();
        query = Include(query, includeProperties);
        return await query.FirstOrDefaultAsync(filter);
    }

    public void Add(T entity) => _dbSet.Add(entity);
    public void Remove(T entity) => _dbSet.Remove(entity);
    public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

    private static IQueryable<T> Include(IQueryable<T> query, string? includeProperties)
    {
        if (string.IsNullOrWhiteSpace(includeProperties)) return query;
        foreach (var prop in includeProperties.Split(',',
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            query = query.Include(prop);
        }
        return query;
    }
}
