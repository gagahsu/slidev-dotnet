using EShop.DataAccess.Data;
using EShop.DataAccess.Repository;
using EShop.Models;
using EShop.Utility;
using EShop.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace EShop.Tests;

// 結帳的整合測試：真的資料庫（SQLite）、真的交易
public class OrderServiceTests : IDisposable
{
    private readonly SqliteTestDb _testDb = new();

    public OrderServiceTests()
    {
        using var db = _testDb.CreateContext();
        db.Users.AddRange(
            new ApplicationUser { Id = "amy", UserName = "amy@eshop.com", Name = "Amy" },
            new ApplicationUser { Id = "ben", UserName = "ben@eshop.com", Name = "Ben" });
        db.SaveChanges();
    }

    [Fact]
    public async Task 下單成功_扣庫存_建立訂單_清空購物車()
    {
        AddToCart("amy", (productId: 1, count: 2), (productId: 2, count: 1));   // 450×2 + 380

        var result = await PlaceOrderAsync("amy");

        Assert.True(result.Succeeded);
        using var db = _testDb.CreateContext();
        var order = await db.OrderHeaders.Include(o => o.OrderDetails).SingleAsync();
        Assert.Equal(1280, order.OrderTotal);
        Assert.Equal(150, order.ShippingFee);                 // 未滿 1500：黑貓運費
        Assert.Equal(SD.StatusPending, order.OrderStatus);
        Assert.Equal(2, order.OrderDetails.Count);
        Assert.Equal(18, (await db.Products.FindAsync(1))!.Stock);
        Assert.Equal(14, (await db.Products.FindAsync(2))!.Stock);
        Assert.Empty(db.ShoppingCarts);
    }

    [Fact]
    public async Task 第二項庫存不足_整筆訂單回滾()
    {
        AddToCart("amy", (productId: 1, count: 2), (productId: 3, count: 6));   // 藝伎只剩 5 包

        var result = await PlaceOrderAsync("amy");

        Assert.False(result.Succeeded);
        Assert.Contains("翡翠莊園藝伎", result.Error);
        using var db = _testDb.CreateContext();
        Assert.Equal(20, (await db.Products.FindAsync(1))!.Stock);   // 第一項已扣的庫存也還原了
        Assert.Empty(db.OrderHeaders);
        Assert.Equal(2, await db.ShoppingCarts.CountAsync());       // 購物車保留，讓顧客調整
    }

    [Fact]
    public async Task 兩位顧客搶最後的庫存_只有先到的成功()
    {
        AddToCart("amy", (productId: 3, count: 3));
        AddToCart("ben", (productId: 3, count: 3));

        var first = await PlaceOrderAsync("amy");
        var second = await PlaceOrderAsync("ben");

        Assert.True(first.Succeeded);
        Assert.False(second.Succeeded);
        using var db = _testDb.CreateContext();
        Assert.Equal(2, (await db.Products.FindAsync(3))!.Stock);   // 不會變成負數
    }

    [Fact]
    public async Task 滿1500免運()
    {
        AddToCart("amy", (productId: 3, count: 2));                    // 1200×2

        var result = await PlaceOrderAsync("amy");

        using var db = _testDb.CreateContext();
        var order = await db.OrderHeaders.FindAsync(result.OrderId);
        Assert.Equal(0, order!.ShippingFee);
    }

    [Fact]
    public async Task 購物車是空的_不能下單()
    {
        var result = await PlaceOrderAsync("amy");

        Assert.Equal("購物車是空的", result.Error);
    }

    private void AddToCart(string userId, params (int productId, int count)[] items)
    {
        using var db = _testDb.CreateContext();
        foreach (var (productId, count) in items)
            db.ShoppingCarts.Add(new ShoppingCart { ApplicationUserId = userId, ProductId = productId, Count = count });
        db.SaveChanges();
    }

    private async Task<PlaceOrderResult> PlaceOrderAsync(string userId)
    {
        using ApplicationDbContext db = _testDb.CreateContext();
        var service = new OrderService(new UnitOfWork(db), new BlackCatShipping());
        return await service.PlaceOrderAsync(userId, new OrderHeader
        {
            Name = "Amy", PhoneNumber = "0912-345-678", City = "台北市", StreetAddress = "信義路 1 號",
        });
    }

    public void Dispose() => _testDb.Dispose();
}
