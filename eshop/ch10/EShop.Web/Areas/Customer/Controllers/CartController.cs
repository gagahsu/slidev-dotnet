using System.Security.Claims;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models.ViewModels;
using EShop.Utility;
using EShop.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController(IUnitOfWork unitOfWork, IOrderService orderService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index()
    {
        ShoppingCartVM vm = new()
        {
            CartList = await unitOfWork.ShoppingCart.GetAllAsync(
                filter: c => c.ApplicationUserId == UserId,
                includeProperties: "Product")
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Plus(int cartId)
    {
        var cart = await unitOfWork.ShoppingCart.GetAsync(
            c => c.Id == cartId && c.ApplicationUserId == UserId, includeProperties: "Product", tracked: true);
        if (cart is null) return NotFound();

        if (cart.Count >= cart.Product!.Stock)
            TempData[SD.Error] = "已達庫存上限";
        else
            cart.Count++;

        await unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Minus(int cartId)
    {
        var cart = await unitOfWork.ShoppingCart.GetAsync(
            c => c.Id == cartId && c.ApplicationUserId == UserId, tracked: true);
        if (cart is null) return NotFound();

        if (cart.Count <= 1) unitOfWork.ShoppingCart.Remove(cart);   // 減到 0 就移除
        else cart.Count--;

        await unitOfWork.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartId)
    {
        var cart = await unitOfWork.ShoppingCart.GetAsync(c => c.Id == cartId && c.ApplicationUserId == UserId);
        if (cart is null) return NotFound();

        unitOfWork.ShoppingCart.Remove(cart);
        await unitOfWork.SaveAsync();
        TempData[SD.Success] = "已移除商品";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Summary()
    {
        var user = await unitOfWork.ApplicationUser.GetAsync(u => u.Id == UserId);
        ShoppingCartVM vm = new()
        {
            CartList = await unitOfWork.ShoppingCart.GetAllAsync(
                filter: c => c.ApplicationUserId == UserId, includeProperties: "Product"),
        };
        if (vm.CartList.Count == 0) return RedirectToAction(nameof(Index));

        vm.OrderHeader.Name = user?.Name ?? "";
        vm.OrderHeader.PhoneNumber = user?.PhoneNumber ?? "";
        vm.OrderHeader.City = user?.City ?? "";
        vm.OrderHeader.StreetAddress = user?.StreetAddress ?? "";
        return View(vm);
    }

    [HttpPost, ActionName("Summary"), ValidateAntiForgeryToken]
    public async Task<IActionResult> SummaryPost(ShoppingCartVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.CartList = await unitOfWork.ShoppingCart.GetAllAsync(
                filter: c => c.ApplicationUserId == UserId, includeProperties: "Product");
            return View(vm);
        }

        // 扣庫存、建立訂單、清空購物車：交給 OrderService 在同一個交易完成
        var result = await orderService.PlaceOrderAsync(UserId, vm.OrderHeader);
        if (!result.Succeeded)
        {
            TempData[SD.Error] = result.Error;
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(
            nameof(OrderConfirmation), new { id = result.OrderId });
    }

    public async Task<IActionResult> OrderConfirmation(int id)
    {
        var order = await unitOfWork.OrderHeader.GetAsync(o => o.Id == id && o.ApplicationUserId == UserId);
        return order is null ? NotFound() : View(order);
    }
}
