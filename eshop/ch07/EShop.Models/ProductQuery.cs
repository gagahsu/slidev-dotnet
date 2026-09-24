using System.ComponentModel.DataAnnotations;

namespace EShop.Models;

public enum ProductSort
{
    [Display(Name = "預設排序")] Default,
    [Display(Name = "價格低到高")] PriceAsc,
    [Display(Name = "價格高到低")] PriceDesc,
}

// 商品列表的查詢條件：每個參數都可以省略
public record ProductQuery(
    string? Keyword = null,
    int? CategoryId = null,
    ProductSort Sort = ProductSort.Default,
    int Page = 1,
    int PageSize = 4);
