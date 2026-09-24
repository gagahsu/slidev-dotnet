using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EShop.Tests;

// 整合測試：在記憶體中啟動整個網站，用 HttpClient 發出請求
public class ProductsPageTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task 每個回應都有版本標頭()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("1.0", response.Headers.GetValues("X-EShop-Version").Single());
    }

    [Fact]
    public async Task 商品列表_可以用關鍵字搜尋()
    {
        var html = await _client.GetHtmlAsync("/products?keyword=衣索比亞");

        Assert.Contains("耶加雪菲", html);
        Assert.Contains("西達摩", html);
        Assert.DoesNotContain("曼特寧", html);
    }

    [Fact]
    public async Task 商品列表_第二頁()
    {
        var html = await _client.GetHtmlAsync("/products?page=2");

        Assert.Contains("曼特寧", html);
        Assert.DoesNotContain("耶加雪菲", html);
    }

    [Fact]
    public async Task 商品詳情_找得到回傳200()
    {
        var html = await _client.GetHtmlAsync("/products/4");

        Assert.Contains("巴拿馬", html);
    }

    [Fact]
    public async Task 商品詳情_找不到回傳404()
    {
        var response = await _client.GetAsync("/products/99");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
