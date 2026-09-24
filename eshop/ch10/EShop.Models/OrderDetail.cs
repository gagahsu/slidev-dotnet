using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EShop.Models;

public class OrderDetail
{
    public int Id { get; set; }

    public int OrderHeaderId { get; set; }
    [ForeignKey(nameof(OrderHeaderId))] public OrderHeader? OrderHeader { get; set; }

    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))] public Product? Product { get; set; }

    public int Count { get; set; }
    [Precision(10, 0)] public decimal Price { get; set; }       // 下單當下的單價
}
