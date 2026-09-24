namespace EShop.Web.Services;

public class BlackCatShipping : IShippingService
{
    public string Name => "黑貓宅急便";
    public decimal Calculate(decimal orderTotal) => orderTotal >= 1500 ? 0 : 150;
}
