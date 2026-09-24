using System.Diagnostics;
using System.Security.Claims;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using EShop.Models.ViewModels;
using EShop.Utility;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> AddToCart(int productId, int count)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cart = await unitOfWork.ShoppingCart.GetAsync(
            c => c.ApplicationUserId == userId && c.ProductId == productId, tracked: true);

        if (cart is null)
            unitOfWork.ShoppingCart.Add(new ShoppingCart { ApplicationUserId = userId, ProductId = productId, Count = count });
        else
            cart.Count += count;                 // 已在購物車：增加數量（tracked，SaveAsync 時自動更新）

        await unitOfWork.SaveAsync();
        TempData[SD.Success] = "已加入購物車";
        return RedirectToAction(nameof(Index));
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
