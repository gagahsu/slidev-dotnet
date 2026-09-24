namespace EShop.Models;

// 購物車的一行：哪個商品、買幾包
public record CartLine(Product Product, int Quantity)
{
    public decimal Subtotal => Product.Price * Quantity;
}

// 試算結果：小計、折扣、應付金額
public record PriceQuote(decimal Subtotal, decimal Discount)
{
    public decimal Total => Subtotal - Discount;
}
