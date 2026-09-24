using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models;

public class ShoppingCart
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    [Range(1, 100, ErrorMessage = "數量需介於 1 到 100")]
    public int Count { get; set; }

    public string ApplicationUserId { get; set; } = "";
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser? ApplicationUser { get; set; }

    [NotMapped]
    public decimal Subtotal => (Product?.Price ?? 0) * Count;   // 不存資料庫，顯示用
}
