using EShop.Models;

namespace EShop.DataAccess.Catalog;

// 商品目錄：Controller 只認得這個介面，不知道資料放在哪裡
public interface IProductCatalog
{
    List<Category> GetCategories();
    Product? GetById(int id);
    PagedResult<Product> Search(ProductQuery query);
    List<CategorySummary> GetCategorySummaries();
}
