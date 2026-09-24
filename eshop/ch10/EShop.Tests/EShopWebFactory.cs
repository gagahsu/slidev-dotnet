using EShop.DataAccess.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EShop.Tests;

// 整合測試用的網站：把 SQL Server 換成 SQLite in-memory，並加上測試用驗證
public class EShopWebFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        // 網站啟動時 DbInitializer 就會用到資料庫，所以要先建好資料表
        using (var db = SqliteTestDb.Create(_connection))
        {
            db.Database.EnsureCreated();
        }

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(o =>
                SqliteTestDb.Configure(o, _connection));

            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, null);
            services.PostConfigure<AuthenticationOptions>(o =>
                o.DefaultAuthenticateScheme = TestAuthHandler.SchemeName);
        });
    }

    // 以指定身分發出請求；不自動轉址，才看得到 302
    public HttpClient CreateClientAs(string? user)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        if (user is not null) client.DefaultRequestHeaders.Add(TestAuthHandler.Header, user);
        return client;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }
}
