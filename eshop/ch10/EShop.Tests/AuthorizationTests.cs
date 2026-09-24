using System.Net;

namespace EShop.Tests;

// 權限矩陣：每種身分打每個後台頁面，確認結果符合 policy
public class AuthorizationTests(EShopWebFactory factory) : IClassFixture<EShopWebFactory>
{
    private const string Ok = "200";
    private const string Login = "登入頁";
    private const string Denied = "拒絕存取";

    [Theory]
    // 身分（角色;分店）    網址                  預期
    [InlineData(null, "/Admin/Dashboard", Login)]
    [InlineData("Customer", "/Admin/Dashboard", Denied)]
    [InlineData("Employee", "/Admin/Dashboard", Denied)]      // 員工沒有分店
    [InlineData("Employee;1", "/Admin/Dashboard", Ok)]
    [InlineData("Employee;1", "/Admin/Product", Ok)]
    [InlineData("Employee;1", "/Admin/Category", Denied)]
    [InlineData("Employee;1", "/Admin/Store", Denied)]
    [InlineData("Admin", "/Admin/Dashboard", Ok)]
    [InlineData("Admin", "/Admin/Category", Ok)]
    [InlineData("Admin", "/Admin/Store", Ok)]
    public async Task 後台頁面依policy授權(string? user, string url, string expected)
    {
        var response = await factory.CreateClientAs(user).GetAsync(url);

        var actual = response.StatusCode switch
        {
            HttpStatusCode.OK => Ok,
            HttpStatusCode.Redirect when IsRedirectTo(response, "/Account/Login") => Login,
            HttpStatusCode.Redirect when IsRedirectTo(response, "/Account/AccessDenied") => Denied,
            var code => code.ToString(),
        };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task 註冊頁_顯示客製化欄位()
    {
        var html = await factory.CreateClient().GetHtmlAsync("/Identity/Account/Register");

        Assert.Contains("姓名", html);
        Assert.DoesNotContain("-- 選擇角色 --", html);   // 只有 Admin 看得到角色欄位
    }

    private static bool IsRedirectTo(HttpResponseMessage response, string path) =>
        response.Headers.Location?.ToString().Contains(path) == true;
}
