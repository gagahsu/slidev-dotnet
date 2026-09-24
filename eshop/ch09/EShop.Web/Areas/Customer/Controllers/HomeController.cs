using System.Diagnostics;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController(IUnitOfWork unitOfWork) : Controller
{
    public async Task<IActionResult> Index()
    {
        var products = await unitOfWork.Product.GetAllAsync(
            filter: p => p.Stock > 0,
            orderBy: q => q.OrderBy(p => p.CategoryId).ThenBy(p => p.Price),
            includeProperties: "Category");
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await unitOfWork.Product.GetAsync(p => p.Id == id, includeProperties: "Category");
        return product is null ? NotFound() : View(product);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
