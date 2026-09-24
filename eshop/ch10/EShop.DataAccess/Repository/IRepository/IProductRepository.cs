using EShop.Models;

namespace EShop.DataAccess.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product product);
    Task<PagedResult<Product>> SearchAsync(ProductQuery query);
    Task<List<CategorySummary>> GetCategorySummariesAsync();
    Task<bool> DecreaseStockAsync(int productId, int count);
    Task IncreaseStockAsync(int productId, int count);
}
