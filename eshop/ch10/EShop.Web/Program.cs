using EShop.DataAccess.Data;
using EShop.DataAccess.DbInitializer;
using EShop.DataAccess.Repository;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using EShop.Utility;
using EShop.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ① 註冊服務
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Lockout.MaxFailedAccessAttempts = 5;     // 連續錯 5 次鎖定
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
builder.Services.AddRazorPages();                 // Identity UI 是 Razor Pages

// 授權規則集中在這裡：Controller 只寫 policy 名稱
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(SD.Policy_Admin, p => p.RequireRole(SD.Role_Admin))
    .AddPolicy(SD.Policy_Staff, p => p.RequireAssertion(context =>
        context.User.IsInRole(SD.Role_Admin) ||
        (context.User.IsInRole(SD.Role_Employee) &&
         context.User.HasClaim(c => c.Type == SD.Claim_StoreId))));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// 有人要 IShippingService 時，給他 BlackCatShipping
builder.Services.AddScoped<IShippingService, BlackCatShipping>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider);
}

// ② 設定 Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Customer/Home/Error");
    app.UseHsts();
}

// EShop 的第一個 Middleware：加上版本標頭、記錄處理時間
app.Use(async (context, next) =>
{
    var start = DateTime.Now;
    // 標頭要在 next() 之前設定：回應開始送出後就不能再改
    context.Response.Headers["X-EShop-Version"] = "1.0";

    await next(context);

    var ms = (DateTime.Now - start).TotalMilliseconds;
    Console.WriteLine(
        $"[EShop] {context.Request.Method} {context.Request.Path}" +
        $" → {context.Response.StatusCode}（{ms:F0} ms）");
});

app.UseHttpsRedirection();
app.UseStaticFiles();      // 服務執行時才上傳的商品圖片
app.UseRouting();

app.UseAuthentication();   // ① 你是誰
app.UseAuthorization();    // ② 你能做什麼

app.MapStaticAssets();
app.MapRazorPages();       // 對應 /Identity/Account/... 頁面

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
