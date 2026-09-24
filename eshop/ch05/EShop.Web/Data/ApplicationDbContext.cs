using EShop.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 最後一道防線：資料庫層級的唯一索引
        modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "單品咖啡豆", DisplayOrder = 1 },
            new Category { Id = 2, Name = "精品咖啡豆", DisplayOrder = 2 },
            new Category { Id = 3, Name = "配方豆", DisplayOrder = 3 });
    }
}
