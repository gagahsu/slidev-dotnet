namespace EShop.Web.Models;

public enum ProductSort
{
    Default,     // 依編號
    PriceAsc,    // 價格低到高
    PriceDesc,   // 價格高到低
}

// 商品列表的查詢條件：每個參數都可以省略
public record ProductQuery(
    string? Keyword = null,
    int? CategoryId = null,
    ProductSort Sort = ProductSort.Default,
    int Page = 1,
    int PageSize = 4);
