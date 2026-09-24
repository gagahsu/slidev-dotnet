namespace EShop.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Origin { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    // C# 14 的 field 關鍵字：庫存永遠不會小於 0
    public int Stock
    {
        get;
        set => field = Math.Max(0, value);
    }

    public bool InStock => Stock > 0;
}
