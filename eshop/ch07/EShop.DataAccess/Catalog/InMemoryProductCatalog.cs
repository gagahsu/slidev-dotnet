using EShop.Models;

namespace EShop.DataAccess.Catalog;

public class InMemoryProductCatalog(List<Product> products, List<Category> categories)
    : IProductCatalog
{
    public List<Category> GetCategories() =>
        categories.OrderBy(c => c.DisplayOrder).ToList();

    public Product? GetById(int id) => products.FirstOrDefault(p => p.Id == id);

    public PagedResult<Product> Search(ProductQuery query)
    {
        IEnumerable<Product> result = products;

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            result = result.Where(p => p.Name.Contains(query.Keyword)
                                    || p.Origin.Contains(query.Keyword));
        }
        if (query.CategoryId is int categoryId)
        {
            result = result.Where(p => p.CategoryId == categoryId);
        }

        result = query.Sort switch
        {
            ProductSort.PriceAsc => result.OrderBy(p => p.Price),
            ProductSort.PriceDesc => result.OrderByDescending(p => p.Price),
            _ => result.OrderBy(p => p.Id),
        };

        var page = Math.Max(1, query.Page);
        var matched = result.ToList();   // 立即執行，只篩選一次
        var items = matched.Skip((page - 1) * query.PageSize)
                           .Take(query.PageSize)
                           .ToList();
        return new PagedResult<Product>(items, page, query.PageSize, matched.Count);
    }

    public List<CategorySummary> GetCategorySummaries() =>
        products.GroupBy(p => p.CategoryId)
                .Select(g => new CategorySummary(
                    GetCategoryName(g.Key),
                    g.Count(),
                    Math.Round(g.Average(p => p.Price)),
                    g.Sum(p => p.Stock)))
                .OrderByDescending(s => s.ProductCount)
                .ToList();

    private string GetCategoryName(int id) =>
        categories.FirstOrDefault(c => c.Id == id)?.Name ?? "未分類";
}
