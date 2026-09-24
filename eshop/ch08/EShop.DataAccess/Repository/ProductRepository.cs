using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.DataAccess.Repository;

public class ProductRepository(ApplicationDbContext db)
    : Repository<Product>(db), IProductRepository
{
    public void Update(Product product) => _db.Products.Update(product);

    // 第 3 章的查詢搬進資料庫：IQueryable 一路串條件，最後才產生 SQL
    public async Task<PagedResult<Product>> SearchAsync(ProductQuery query)
    {
        IQueryable<Product> products = _db.Products.AsNoTracking()
                                                   .Include(p => p.Category);

        if (query.Keyword is { Length: > 0 } keyword)
        {
            products = products.Where(p => p.Name.Contains(keyword)
                                        || p.Origin.Contains(keyword));
        }
        if (query.CategoryId is int categoryId)
        {
            products = products.Where(p => p.CategoryId == categoryId);
        }

        products = query.Sort switch
        {
            ProductSort.PriceAsc => products.OrderBy(p => p.Price),
            ProductSort.PriceDesc => products.OrderByDescending(p => p.Price),
            _ => products.OrderBy(p => p.Id),
        };

        var page = Math.Max(1, query.Page);
        var total = await products.CountAsync();            // SELECT COUNT(*)
        var items = await products.Skip((page - 1) * query.PageSize)
                                  .Take(query.PageSize)
                                  .ToListAsync();           // OFFSET ... FETCH
        return new PagedResult<Product>(items, page, query.PageSize, total);
    }

    // GroupBy 也交給資料庫：GROUP BY + COUNT、AVG、SUM
    public Task<List<CategorySummary>> GetCategorySummariesAsync() =>
        _db.Products
           .GroupBy(p => p.Category!.Name)
           .OrderByDescending(g => g.Count())
           .Select(g => new CategorySummary(
               g.Key, g.Count(), g.Average(p => p.Price), g.Sum(p => p.Stock)))
           .ToListAsync();
}
