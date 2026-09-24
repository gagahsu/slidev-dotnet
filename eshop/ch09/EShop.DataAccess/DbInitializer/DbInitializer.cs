using EShop.DataAccess.Data;
using EShop.Models;
using EShop.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.DataAccess.DbInitializer;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();                       // 自動套用尚未執行的 Migration

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { SD.Role_Admin, SD.Role_Employee, SD.Role_Customer })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        if (!await db.Stores.AnyAsync())
        {
            db.Stores.AddRange(
                new Store { Name = "信義店", City = "台北市", StreetAddress = "信義路五段 7 號", PhoneNumber = "02-2345-6789" },
                new Store { Name = "勤美店", City = "台中市", StreetAddress = "公益路 68 號", PhoneNumber = "04-2321-0000" });
            await db.SaveChangesAsync();
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await CreateUserAsync(userManager, "admin@eshop.com", "系統管理員", SD.Role_Admin);

        // EShop 延伸：一位有分店的員工，用來示範 Staff policy
        var storeId = (await db.Stores.OrderBy(s => s.Id).FirstAsync()).Id;
        await CreateUserAsync(userManager, "employee@eshop.com", "信義店店員",
                              SD.Role_Employee, storeId);
    }

    private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager,
        string email, string name, string role, int? storeId = null)
    {
        if (await userManager.FindByEmailAsync(email) is not null) return;

        var user = new ApplicationUser
        {
            UserName = email, Email = email, EmailConfirmed = true,
            Name = name, StoreId = storeId,
        };
        await userManager.CreateAsync(user, "Admin@1234");
        await userManager.AddToRoleAsync(user, role);
    }
}
