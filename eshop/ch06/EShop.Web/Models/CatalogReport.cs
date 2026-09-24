namespace EShop.Web.Models;

// 依分類統計：款數、平均價格、總庫存
public record CategorySummary(
    string CategoryName, int ProductCount, decimal AveragePrice, int TotalStock);
