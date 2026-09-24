using EShop.DataAccess.Repository;
using EShop.Models;
using EShop.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace EShop.Tests;

public class CategoryControllerTests : IDisposable
{
    private readonly SqliteTestDb _testDb = new();

    private CategoryController CreateController() =>
        new(new UnitOfWork(_testDb.CreateContext()))
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), new NullTempDataProvider()),
        };

    [Theory]
    [InlineData("配方豆")]
    [InlineData("  配方豆 ")]
    public async Task Create_名稱重複_回到表單(string name)
    {
        var controller = CreateController();
        var category = new Category { Name = name, DisplayOrder = 9 };

        var result = await controller.Create(category);

        Assert.IsType<ViewResult>(result);
        Assert.True(controller.ModelState.ContainsKey(nameof(Category.Name)));
        Assert.Equal(3, await _testDb.CreateContext().Categories.CountAsync());
    }

    [Fact]
    public async Task Create_新名稱_存檔並顯示通知()
    {
        var controller = CreateController();

        var result = await controller.Create(new Category { Name = "濾掛咖啡", DisplayOrder = 4 });

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("分類新增成功", controller.TempData["success"]);
        Assert.Equal(4, await _testDb.CreateContext().Categories.CountAsync());
    }

    [Fact]
    public async Task Edit_保留自己的名稱_可以更新()
    {
        var controller = CreateController();

        var result = await controller.Edit(new Category { Id = 3, Name = "配方豆", DisplayOrder = 5 });

        Assert.IsType<RedirectToActionResult>(result);
        var saved = await _testDb.CreateContext().Categories.FindAsync(3);
        Assert.Equal(5, saved!.DisplayOrder);
    }

    [Fact]
    public async Task Edit_改成別人的名稱_回到表單()
    {
        var controller = CreateController();
        var category = new Category { Id = 3, Name = "單品咖啡豆", DisplayOrder = 3 };

        var result = await controller.Edit(category);

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task 唯一索引_資料庫也會擋下重複名稱()
    {
        using var db = _testDb.CreateContext();
        db.Categories.Add(new Category { Name = "配方豆", DisplayOrder = 9 });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    public void Dispose() => _testDb.Dispose();

    private class NullTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) =>
            new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}
