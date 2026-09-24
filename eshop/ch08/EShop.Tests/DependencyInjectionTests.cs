using EShop.Web.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Tests;

// DI 的好處：測試時可以把零件換成別的，Controller 與 View 一行都不用改
public class DependencyInjectionTests(EShopWebFactory factory)
    : IClassFixture<EShopWebFactory>
{
    [Fact]
    public async Task 詳情頁_顯示目前註冊的物流商運費()
    {
        var html = await factory.CreateClient().GetHtmlAsync("/Customer/Home/Details/1");

        Assert.Contains("黑貓宅急便 運費 150 元", html);
    }

    [Fact]
    public async Task 換掉物流商_View不用改()
    {
        var client = factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
                // 後註冊的會蓋掉先註冊的
                services.AddScoped<IShippingService, ConvenienceStoreShipping>()))
            .CreateClient();

        var html = await client.GetHtmlAsync("/Customer/Home/Details/1");

        Assert.Contains("超商取貨 運費 60 元", html);
    }

    [Fact]
    public void 生命週期_Scoped每個請求各一個()
    {
        using var scope1 = factory.Services.CreateScope();
        using var scope2 = factory.Services.CreateScope();

        var shipping1 = scope1.ServiceProvider.GetRequiredService<IShippingService>();
        var shipping1Again = scope1.ServiceProvider.GetRequiredService<IShippingService>();
        var shipping2 = scope2.ServiceProvider.GetRequiredService<IShippingService>();

        Assert.Same(shipping1, shipping1Again);   // 同一個請求內相同
        Assert.NotSame(shipping1, shipping2);     // 不同請求各一個
    }
}
