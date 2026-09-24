using EShop.Web.Models;
using EShop.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Controllers;

[Route("products")]
public class ProductsController(IProductCatalog catalog, IShippingService shipping)
    : Controller
{
    // GET /products?keyword=衣索比亞&categoryId=1&sort=PriceAsc&page=2
    [HttpGet("")]
    public IActionResult Index(ProductQuery query)
    {
        ViewData["Query"] = query;
        ViewData["Categories"] = catalog.GetCategories();
        return View(catalog.Search(query));
    }

    // GET /products/4
    [HttpGet("{id:int}")]
    public IActionResult Details(int id)
    {
        var product = catalog.GetById(id);
        if (product is null)
        {
            return NotFound();
        }

        // 運費試算：單買一包要付多少運費
        ViewData["ShippingName"] = shipping.Name;
        ViewData["ShippingFee"] = shipping.Calculate(product.Price);
        return View(product);
    }
}
