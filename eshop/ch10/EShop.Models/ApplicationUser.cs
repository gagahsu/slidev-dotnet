using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EShop.Models;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(30), Display(Name = "姓名")]
    public string Name { get; set; } = "";

    [Display(Name = "縣市")] public string? City { get; set; }
    [Display(Name = "地址")] public string? StreetAddress { get; set; }
    [Display(Name = "郵遞區號")] public string? PostalCode { get; set; }

    // 所屬分店（員工才需要，所以可為 null）
    public int? StoreId { get; set; }
    [ForeignKey(nameof(StoreId))] public Store? Store { get; set; }
}
