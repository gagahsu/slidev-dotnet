using EShop.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Controllers;

public class CheckoutController(IShippingService shipping) : Controller
{
    public IActionResult Index(decimal total = 1000)
    {
        var fee = shipping.Calculate(total);
        return Content($"{shipping.Name}：訂單 {total} 元，運費 {fee} 元");
    }
}
