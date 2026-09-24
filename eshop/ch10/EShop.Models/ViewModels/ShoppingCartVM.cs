namespace EShop.Models.ViewModels;

public class ShoppingCartVM
{
    public List<ShoppingCart> CartList { get; set; } = [];
    public OrderHeader OrderHeader { get; set; } = new();       // 收件資訊與金額

    public decimal OrderTotal => CartList.Sum(c => c.Subtotal);
    public decimal Shipping => OrderTotal >= 1500 ? 0 : 150;
    public decimal AmountDue => OrderTotal + Shipping;
}
