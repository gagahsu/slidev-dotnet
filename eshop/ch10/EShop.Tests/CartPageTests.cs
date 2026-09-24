using System.Net;

namespace EShop.Tests;

public class CartPageTests(EShopWebFactory factory) : IClassFixture<EShopWebFactory>
{
    [Fact]
    public async Task 購物車_未登入會轉到登入頁()
    {
        var response = await factory.CreateClientAs(null).GetAsync("/Customer/Cart");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task 購物車_登入後顯示空的購物車()
    {
        var html = await factory.CreateClientAs("Customer;;nobody").GetHtmlAsync("/Customer/Cart");

        Assert.Contains("購物車是空的", html);
    }

    [Theory]
    [InlineData("Employee;1", HttpStatusCode.OK)]
    [InlineData("Customer", HttpStatusCode.Redirect)]   // 轉到拒絕存取
    public async Task 訂單管理_只有員工與管理員能進入(string user, HttpStatusCode expected)
    {
        var response = await factory.CreateClientAs(user).GetAsync("/Admin/Order/GetAll");

        Assert.Equal(expected, response.StatusCode);
    }
}
