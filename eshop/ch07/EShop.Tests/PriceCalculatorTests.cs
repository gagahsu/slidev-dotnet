using EShop.Models;
using EShop.Web.Services;

namespace EShop.Tests;

public class PriceCalculatorTests
{
    [Theory]
    [InlineData(MemberLevel.Regular, 1000, 0)]
    [InlineData(MemberLevel.Silver, 1000, 50)]
    [InlineData(MemberLevel.Gold, 1000, 100)]
    [InlineData(MemberLevel.Gold, 2000, 300)]
    [InlineData(MemberLevel.Gold, 0, 0)]
    public void GetDiscount_依會員等級計算折扣(
        MemberLevel level, int subtotal, int expected)
    {
        var calculator = new PriceCalculator(level);

        var discount = calculator.GetDiscount(subtotal);

        Assert.Equal(expected, discount);
    }

    [Fact]
    public void Quote_加總每一行再套用折扣()
    {
        var yirga = new Product { Name = "耶加雪菲", Origin = "衣索比亞", Price = 450 };
        var villa = new Product { Name = "薇拉", Origin = "哥倫比亞", Price = 380 };
        List<CartLine> lines = [new(yirga, 2), new(villa, 1)];
        var calculator = new PriceCalculator(MemberLevel.Silver);

        var quote = calculator.Quote(lines);

        Assert.Equal(1280, quote.Subtotal);
        Assert.Equal(64, quote.Discount);
        Assert.Equal(1216, quote.Total);
    }

    [Fact]
    public void Stock_不能小於零()
    {
        var bean = new Product { Name = "西達摩", Origin = "衣索比亞", Stock = 5 };

        bean.Stock -= 10;

        Assert.Equal(0, bean.Stock);
        Assert.False(bean.InStock);
    }
}
