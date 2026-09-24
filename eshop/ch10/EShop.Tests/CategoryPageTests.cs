namespace EShop.Tests;

public class CategoryPageTests(EShopWebFactory factory) : IClassFixture<EShopWebFactory>
{
    [Fact]
    public async Task 分類列表_從資料庫讀出種子資料()
    {
        var html = await factory.CreateClientAs("Admin").GetHtmlAsync("/Admin/Category");

        Assert.Contains("單品咖啡豆", html);
        Assert.Contains("配方豆", html);
    }
}
