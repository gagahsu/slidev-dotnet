using System.Security.Claims;
using System.Text.Encodings.Web;
using EShop.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EShop.Tests;

// 測試用的驗證：從標頭讀出「角色;分店編號;使用者Id」，不必真的登入
public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    public const string Header = "X-Test-User";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Header, out var value))
            return Task.FromResult(AuthenticateResult.NoResult());   // 沒帶標頭 = 匿名

        var parts = value.ToString().Split(';');
        List<Claim> claims =
        [
            new(ClaimTypes.Name, "tester"),
            new(ClaimTypes.Role, parts[0]),
            new(ClaimTypes.NameIdentifier, parts.ElementAtOrDefault(2) ?? "test-user"),
        ];
        if (parts.Length > 1 && parts[1] != "") claims.Add(new(SD.Claim_StoreId, parts[1]));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }
}
