using System.Security.Claims;
using EShop.Models;
using EShop.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EShop.Web.Services;

// 登入時把「所屬分店」寫進 Cookie，授權時就不用再查資料庫
public class ApplicationUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(
        userManager, roleManager, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        if (user.StoreId is int storeId)
        {
            identity.AddClaim(new Claim(SD.Claim_StoreId, storeId.ToString()));
        }
        return identity;
    }
}
