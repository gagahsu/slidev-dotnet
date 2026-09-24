using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Route("products")]
public class ProductsController(IUnitOfWork unitOfWork) : Controller
{
    // GET /products?keyword=衣索比亞&categoryId=1&sort=PriceAsc&page=2
    [HttpGet("")]
    public async Task<IActionResult> Index(ProductQuery query)
    {
        ViewData["Query"] = query;
        ViewData["Categories"] = await unitOfWork.Category.GetAllAsync(
            orderBy: q => q.OrderBy(c => c.DisplayOrder));
        return View(await unitOfWork.Product.SearchAsync(query));
    }
}
