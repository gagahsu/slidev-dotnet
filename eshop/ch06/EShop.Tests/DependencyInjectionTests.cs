using EShop.Web.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Tests;

// DI 的好處：測試時可以把零件換成假的，Controller 一行都不用改
public class DependencyInjectionTests(EShopWebFactory factory)
    : IClassFixture<EShopWebFactory>
{
    [Fact]
    public async Task 詳情頁_顯示目前註冊的物流商運費()
    {
        var html = await factory.CreateClient().GetHtmlAsync("/products/1");

        Assert.Contains("黑貓宅急便 運費 150 元", html);
    }

    [Fact]
    public async Task 換掉物流商與目錄_Controller不用改()
    {
        var client = factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                // 後註冊的會蓋掉先註冊的
                services.AddScoped<IShippingService, ConvenienceStoreShipping>();
                services.AddSingleton<IProductCatalog>(new InMemoryProductCatalog(
                    [new() { Id = 1, Name = "測試豆", Origin = "測試", Price = 100 }],
                    [new() { Id = 1, Name = "測試分類" }]));
            })).CreateClient();

        var html = await client.GetHtmlAsync("/products/1");

        Assert.Contains("測試豆", html);
        Assert.Contains("超商取貨 運費 60 元", html);
    }

    [Fact]
    public void 生命週期_Singleton整個網站共用同一份()
    {
        using var scope1 = factory.Services.CreateScope();
        using var scope2 = factory.Services.CreateScope();

        var catalog1 = scope1.ServiceProvider.GetRequiredService<IProductCatalog>();
        var catalog2 = scope2.ServiceProvider.GetRequiredService<IProductCatalog>();
        var shipping1 = scope1.ServiceProvider.GetRequiredService<IShippingService>();
        var shipping2 = scope2.ServiceProvider.GetRequiredService<IShippingService>();

        Assert.Same(catalog1, catalog2);         // Singleton：同一個
        Assert.NotSame(shipping1, shipping2);    // Scoped：每個請求各一個
    }
}
