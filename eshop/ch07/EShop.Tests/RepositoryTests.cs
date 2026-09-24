using EShop.DataAccess.Repository;
using EShop.Models;

namespace EShop.Tests;

// Repository 與 UnitOfWork：不經過網站，直接測資料存取層
public class RepositoryTests : IDisposable
{
    private readonly SqliteTestDb _testDb = new();

    [Fact]
    public async Task GetAllAsync_篩選與排序都在資料庫執行()
    {
        var unitOfWork = new UnitOfWork(_testDb.CreateContext());

        var categories = await unitOfWork.Category.GetAllAsync(
            filter: c => c.DisplayOrder >= 2,
            orderBy: q => q.OrderByDescending(c => c.DisplayOrder));

        Assert.Equal(["配方豆", "精品咖啡豆"], categories.Select(c => c.Name));
    }

    [Theory]
    [InlineData("配方豆", 0, true)]    // 新增：和既有分類重複
    [InlineData("配方豆", 3, false)]   // 編輯自己：不算重複
    [InlineData("濾掛咖啡", 0, false)]
    public async Task IsNameExistsAsync_檢查名稱是否重複(
        string name, int excludeId, bool expected)
    {
        var unitOfWork = new UnitOfWork(_testDb.CreateContext());

        Assert.Equal(expected, await unitOfWork.Category.IsNameExistsAsync(name, excludeId));
    }

    [Fact]
    public async Task SaveAsync_呼叫之前不會寫入資料庫()
    {
        var unitOfWork = new UnitOfWork(_testDb.CreateContext());
        unitOfWork.Category.Add(new Category { Name = "濾掛咖啡", DisplayOrder = 4 });

        var before = await new UnitOfWork(_testDb.CreateContext()).Category.GetAllAsync();
        await unitOfWork.SaveAsync();
        var after = await new UnitOfWork(_testDb.CreateContext()).Category.GetAllAsync();

        Assert.Equal(3, before.Count);
        Assert.Equal(4, after.Count);
    }

    public void Dispose() => _testDb.Dispose();
}
