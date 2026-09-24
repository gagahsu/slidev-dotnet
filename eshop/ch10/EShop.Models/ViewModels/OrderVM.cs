namespace EShop.Models.ViewModels;

public class OrderVM
{
    public OrderHeader OrderHeader { get; set; } = new();
    public List<OrderDetail> OrderDetails { get; set; } = [];
}
