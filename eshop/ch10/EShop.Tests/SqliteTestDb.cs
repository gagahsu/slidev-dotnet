using EShop.DataAccess.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Tests;

// 測試用資料庫：SQLite in-memory，連線關閉資料就消失
public sealed class SqliteTestDb : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public SqliteTestDb()
    {
        _connection.Open();
        using var db = CreateContext();
        db.Database.EnsureCreated();   // 依模型建表（含 HasData 種子資料）
    }

    public ApplicationDbContext CreateContext() => Create(_connection);

    public static ApplicationDbContext Create(SqliteConnection connection)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        Configure(builder, connection);
        return new ApplicationDbContext(builder.Options);
    }

    // 網站與單元測試共用的 SQLite 設定
    public static DbContextOptionsBuilder Configure(
        DbContextOptionsBuilder builder, SqliteConnection connection) =>
        builder.UseSqlite(connection)
               .ReplaceService<IModelCustomizer, SqliteModelCustomizer>()
               // Migration 是為 SQL Server 產生的：測試改用 EnsureCreated，MigrateAsync 什麼都不做
               .ReplaceService<IMigrationsAssembly, NoMigrationsAssembly>()
               .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

    public void Dispose() => _connection.Dispose();
}
