using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace EShop.Models;

public class OrderHeader
{
    public int Id { get; set; }
    [ValidateNever] public string ApplicationUserId { get; set; } = "";   // 由伺服器填入，不驗證表單
    [ForeignKey(nameof(ApplicationUserId))] public ApplicationUser? ApplicationUser { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    [Precision(10, 0)] public decimal OrderTotal { get; set; }
    [Precision(10, 0)] public decimal ShippingFee { get; set; }

    public string OrderStatus { get; set; } = SD.StatusPending;
    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }

    [Required, Display(Name = "收件人")] public string Name { get; set; } = "";
    [Required, Display(Name = "電話")] public string PhoneNumber { get; set; } = "";
    [Required, Display(Name = "縣市")] public string City { get; set; } = "";
    [Required, Display(Name = "地址")] public string StreetAddress { get; set; } = "";

    public List<OrderDetail> OrderDetails { get; set; } = [];
}
