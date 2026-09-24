using EShop.DataAccess.Repository;
using EShop.Models;

namespace EShop.Tests;

// 第 3 章的查詢測試，現在改由資料庫執行
public class ProductRepositoryTests : IDisposable
{
    private readonly SqliteTestDb _testDb = new();

    private ProductRepository CreateRepository() => new(_testDb.CreateContext());

    [Fact]
    public async Task SearchAsync_沒有條件_依編號回傳第一頁()
    {
        var result = await CreateRepository().SearchAsync(new ProductQuery());

        Assert.Equal([1, 2, 3, 4], result.Items.Select(p => p.Id));
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task SearchAsync_關鍵字比對名稱或產地_並載入分類()
    {
        var result = await CreateRepository().SearchAsync(new ProductQuery(Keyword: "衣索比亞"));

        Assert.Equal(["耶加雪菲 G1", "西達摩"], result.Items.Select(p => p.Name));
        Assert.All(result.Items, p => Assert.Equal("單品咖啡豆", p.Category?.Name));
    }

    [Fact]
    public async Task SearchAsync_分類篩選加價格排序()
    {
        var query = new ProductQuery(CategoryId: 1, Sort: ProductSort.PriceDesc);

        var result = await CreateRepository().SearchAsync(query);

        Assert.Equal([450m, 420m, 400m, 380m], result.Items.Select(p => p.Price));
    }

    [Fact]
    public async Task SearchAsync_第二頁只剩兩筆()
    {
        var result = await CreateRepository().SearchAsync(new ProductQuery(Page: 2));

        Assert.Equal(["曼特寧", "綜合配方豆"], result.Items.Select(p => p.Name));
        Assert.False(result.HasNext);
    }

    [Fact]
    public async Task GetCategorySummariesAsync_在資料庫分組統計()
    {
        var summaries = await CreateRepository().GetCategorySummariesAsync();

        var single = summaries[0];
        Assert.Equal("單品咖啡豆", single.CategoryName);
        Assert.Equal(4, single.ProductCount);
        Assert.Equal(412.5m, single.AveragePrice);
        Assert.Equal(65, single.TotalStock);
    }

    public void Dispose() => _testDb.Dispose();
}
