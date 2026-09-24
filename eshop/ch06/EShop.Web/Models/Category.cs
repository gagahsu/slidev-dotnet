using System.ComponentModel.DataAnnotations;

namespace EShop.Web.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "請輸入分類名稱")]
    [MaxLength(30, ErrorMessage = "分類名稱最多 30 個字")]
    [Display(Name = "分類名稱")]
    public string Name { get; set; } = "";

    [Range(1, 100, ErrorMessage = "顯示順序必須介於 1 到 100")]
    [Display(Name = "顯示順序")]
    public int DisplayOrder { get; set; }
}
