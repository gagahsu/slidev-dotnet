using System.Net;

namespace EShop.Tests;

public class AreaPageTests(EShopWebFactory factory) : IClassFixture<EShopWebFactory>
{
    private readonly HttpClient _client = factory.CreateClientAs("Admin");

    [Theory]
    [InlineData("/")]                      // 預設 Area 是 Customer
    [InlineData("/Customer/Home/Privacy")]
    [InlineData("/products")]              // attribute route 在 Area 中照樣運作
    [InlineData("/Admin/Category")]
    [InlineData("/Admin/Dashboard")]
    [InlineData("/Admin/Product")]
    [InlineData("/Admin/Product/Upsert")]
    [InlineData("/Admin/Product/Upsert/1")]
    public async Task 前台與後台頁面都能開啟(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task 營運總覽_顯示分類統計()
    {
        var html = await _client.GetHtmlAsync("/Admin/Dashboard");

        Assert.Contains("資料庫中共有 <strong>3</strong> 個分類", html);
        Assert.Contains("單品咖啡豆", html);
    }
}
