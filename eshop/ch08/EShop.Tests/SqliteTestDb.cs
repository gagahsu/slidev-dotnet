using EShop.DataAccess.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

    public ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .ReplaceService<IModelCustomizer, SqliteModelCustomizer>()
            .Options);

    public void Dispose() => _connection.Dispose();
}
