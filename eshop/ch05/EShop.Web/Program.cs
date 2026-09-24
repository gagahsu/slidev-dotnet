using EShop.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ① 註冊服務
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ② 設定 Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
