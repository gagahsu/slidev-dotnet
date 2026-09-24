using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "請輸入商品名稱"), MaxLength(50)]
    [Display(Name = "商品名稱")]
    public string Name { get; set; } = "";

    [Display(Name = "商品描述")]
    public string? Description { get; set; }

    [Required, MaxLength(30), Display(Name = "產地")]
    public string Origin { get; set; } = "";

    [Range(1, 100000, ErrorMessage = "價格需介於 1 到 100,000")]
    [Precision(10, 0), Display(Name = "售價")]
    public decimal Price { get; set; }

    [Range(0, 9999), Display(Name = "庫存")]
    public int Stock { get; set; }

    [Display(Name = "分類")]
    public int CategoryId { get; set; }                 // 外鍵

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }             // 導覽屬性

    public string? ImageUrl { get; set; }               // 圖片路徑

    // 只有 get 的屬性不會變成資料表欄位
    public bool InStock => Stock > 0;
}
