namespace EShop.Web.Services;

public interface IShippingService
{
    string Name { get; }
    decimal Calculate(decimal orderTotal);
}
