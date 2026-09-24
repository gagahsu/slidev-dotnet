using EShop.Models;
using EShop.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Tests;

public class IdentitySeedTests(EShopWebFactory factory) : IClassFixture<EShopWebFactory>
{
    [Fact]
    public async Task DbInitializer_建立角色與管理員()
    {
        using var scope = factory.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { SD.Role_Admin, SD.Role_Employee, SD.Role_Customer })
            Assert.True(await roleManager.RoleExistsAsync(role));

        var admin = await userManager.FindByEmailAsync("admin@eshop.com");
        Assert.NotNull(admin);
        Assert.True(await userManager.IsInRoleAsync(admin, SD.Role_Admin));
    }

    [Theory]
    [InlineData("employee@eshop.com", true)]
    [InlineData("admin@eshop.com", false)]
    public async Task 登入時_有分店的使用者會帶StoreId(string email, bool hasStore)
    {
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var claimsFactory = scope.ServiceProvider
            .GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);
        var principal = await claimsFactory.CreateAsync(user!);

        Assert.Equal(hasStore, principal.HasClaim(c => c.Type == SD.Claim_StoreId));
    }
}
