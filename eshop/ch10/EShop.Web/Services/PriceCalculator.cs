using EShop.Models;

namespace EShop.Web.Services;

public class PriceCalculator(MemberLevel level)
{
    // 會員折扣：金卡 9 折、滿 2000 再升級為 85 折；銀卡 95 折
    public decimal GetDiscount(decimal subtotal) => (level, subtotal) switch
    {
        (_, <= 0) => 0,
        (MemberLevel.Gold, >= 2000) => Math.Round(subtotal * 0.15m),
        (MemberLevel.Gold, _) => Math.Round(subtotal * 0.10m),
        (MemberLevel.Silver, _) => Math.Round(subtotal * 0.05m),
        _ => 0,
    };

    public PriceQuote Quote(List<CartLine> lines)
    {
        decimal subtotal = 0;
        foreach (var line in lines)
        {
            subtotal += line.Subtotal;
        }
        return new PriceQuote(subtotal, GetDiscount(subtotal));
    }
}
