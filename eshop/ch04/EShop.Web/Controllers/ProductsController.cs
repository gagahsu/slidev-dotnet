using EShop.Web.Data;
using EShop.Web.Models;
using EShop.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Controllers;

[Route("products")]
public class ProductsController : Controller
{
    private readonly ProductQueryService _catalog =
        new(SeedData.Products, SeedData.Categories);

    // GET /products?keyword=衣索比亞&categoryId=1&sort=PriceAsc&page=2
    [HttpGet("")]
    public IActionResult Index(ProductQuery query)
    {
        ViewData["Query"] = query;
        ViewData["Categories"] = _catalog.GetCategories();
        return View(_catalog.Search(query));
    }

    // GET /products/4
    [HttpGet("{id:int}")]
    public IActionResult Details(int id)
    {
        var product = _catalog.GetById(id);
        if (product is null)
        {
            return NotFound();
        }
        return View(product);
    }
}
