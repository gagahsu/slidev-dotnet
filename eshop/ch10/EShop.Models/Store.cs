using System.ComponentModel.DataAnnotations;

namespace EShop.Models;

public class Store
{
    public int Id { get; set; }

    [Required, MaxLength(30), Display(Name = "分店名稱")]
    public string Name { get; set; } = "";

    [Required, Display(Name = "縣市")] public string City { get; set; } = "";
    [Required, Display(Name = "地址")] public string StreetAddress { get; set; } = "";
    [Display(Name = "電話")] public string? PhoneNumber { get; set; }
}
