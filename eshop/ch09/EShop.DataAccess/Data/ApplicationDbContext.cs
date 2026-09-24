using EShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EShop.DataAccess.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Store> Stores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);      // Identity 的資料表設定在這裡

        // 最後一道防線：資料庫層級的唯一索引
        modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "單品咖啡豆", DisplayOrder = 1 },
            new Category { Id = 2, Name = "精品咖啡豆", DisplayOrder = 2 },
            new Category { Id = 3, Name = "配方豆", DisplayOrder = 3 });

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "耶加雪菲 G1", Origin = "衣索比亞", Price = 450, Stock = 20, CategoryId = 1 },
            new Product { Id = 2, Name = "薇拉 SHB", Origin = "哥倫比亞", Price = 380, Stock = 15, CategoryId = 1 },
            new Product { Id = 3, Name = "翡翠莊園藝伎", Origin = "巴拿馬", Price = 1200, Stock = 5, CategoryId = 2 },
            // EShop 延伸：沿用第 3 章的另外三包豆子，列表才有第二頁
            new Product { Id = 4, Name = "西達摩", Origin = "衣索比亞", Price = 420, Stock = 0, CategoryId = 1 },
            new Product { Id = 5, Name = "曼特寧", Origin = "印尼", Price = 400, Stock = 30, CategoryId = 1 },
            new Product { Id = 6, Name = "綜合配方豆", Origin = "綜合", Price = 299, Stock = 50, CategoryId = 3 });
    }
}
