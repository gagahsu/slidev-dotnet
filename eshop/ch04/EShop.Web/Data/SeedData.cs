using EShop.Web.Models;

namespace EShop.Web.Data;

// 還沒有資料庫之前，先把資料放在記憶體裡
public static class SeedData
{
    public static List<Category> Categories { get; } =
    [
        new() { Id = 1, Name = "單品咖啡豆", DisplayOrder = 1 },
        new() { Id = 2, Name = "精品咖啡豆", DisplayOrder = 2 },
        new() { Id = 3, Name = "配方豆", DisplayOrder = 3 },
    ];

    public static List<Product> Products { get; } =
    [
        new() { Id = 1, Name = "耶加雪菲", Origin = "衣索比亞", Price = 450, Stock = 20, CategoryId = 1 },
        new() { Id = 2, Name = "西達摩", Origin = "衣索比亞", Price = 420, Stock = 0, CategoryId = 1 },
        new() { Id = 3, Name = "薇拉", Origin = "哥倫比亞", Price = 380, Stock = 15, CategoryId = 1 },
        new() { Id = 4, Name = "藝伎", Origin = "巴拿馬", Price = 1200, Stock = 5, CategoryId = 2 },
        new() { Id = 5, Name = "曼特寧", Origin = "印尼", Price = 400, Stock = 30, CategoryId = 1 },
        new() { Id = 6, Name = "綜合配方豆", Origin = "綜合", Price = 299, Stock = 50, CategoryId = 3 },
    ];
}
