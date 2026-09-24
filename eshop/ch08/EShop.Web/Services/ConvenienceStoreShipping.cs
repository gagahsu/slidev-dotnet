namespace EShop.Web.Services;

public class ConvenienceStoreShipping : IShippingService
{
    public string Name => "超商取貨";
    public decimal Calculate(decimal orderTotal) => 60;
}
