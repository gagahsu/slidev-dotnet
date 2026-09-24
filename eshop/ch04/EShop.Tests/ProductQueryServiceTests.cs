using EShop.Web.Data;
using EShop.Web.Models;
using EShop.Web.Services;

namespace EShop.Tests;

public class ProductQueryServiceTests
{
    private readonly ProductQueryService _service =
        new(SeedData.Products, SeedData.Categories);

    [Fact]
    public void Search_沒有條件_依編號回傳第一頁()
    {
        var result = _service.Search(new ProductQuery());

        Assert.Equal([1, 2, 3, 4], result.Items.Select(p => p.Id));
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.True(result.HasNext);
    }

    [Fact]
    public void Search_關鍵字比對名稱或產地()
    {
        var result = _service.Search(new ProductQuery(Keyword: "衣索比亞"));

        Assert.Equal(["耶加雪菲", "西達摩"], result.Items.Select(p => p.Name));
    }

    [Fact]
    public void Search_分類篩選加價格排序()
    {
        var query = new ProductQuery(CategoryId: 1, Sort: ProductSort.PriceDesc);

        var result = _service.Search(query);

        Assert.Equal([450m, 420m, 400m, 380m], result.Items.Select(p => p.Price));
    }

    [Fact]
    public void Search_第二頁只剩兩筆()
    {
        var result = _service.Search(new ProductQuery(Page: 2));

        Assert.Equal(2, result.Items.Count);
        Assert.True(result.HasPrevious);
        Assert.False(result.HasNext);
    }

    [Fact]
    public void GetById_找不到回傳null()
    {
        Assert.Equal("藝伎", _service.GetById(4)?.Name);
        Assert.Null(_service.GetById(99));
    }

    [Fact]
    public void GetCategorySummaries_依分類分組統計()
    {
        var single = _service.GetCategorySummaries()[0];

        Assert.Equal(new CategorySummary("單品咖啡豆", 4, 412, 65), single);
    }
}
