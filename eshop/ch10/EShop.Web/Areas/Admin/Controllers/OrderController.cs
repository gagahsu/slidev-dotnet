using EShop.DataAccess.Repository.IRepository;
using EShop.Models.ViewModels;
using EShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = SD.Policy_Staff)]
public class OrderController(IUnitOfWork unitOfWork) : Controller
{
    public IActionResult Index() => View();

    public async Task<IActionResult> Details(int id)
    {
        var header = await unitOfWork.OrderHeader.GetAsync(o => o.Id == id, includeProperties: "ApplicationUser");
        if (header is null) return NotFound();
        OrderVM vm = new()
        {
            OrderHeader = header,
            OrderDetails = await unitOfWork.OrderDetail.GetAllAsync(d => d.OrderHeaderId == id, includeProperties: "Product")
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> StartProcessing(int id)
    {
        var order = await unitOfWork.OrderHeader.GetAsync(o => o.Id == id);
        if (order?.OrderStatus != SD.StatusPending)
        {
            TempData[SD.Error] = "目前狀態無法轉為處理中";
        }
        else
        {
            await unitOfWork.OrderHeader.UpdateStatusAsync(id, SD.StatusProcessing);
            await unitOfWork.SaveAsync();
            TempData[SD.Success] = "訂單已開始處理";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ShipOrder(OrderVM vm)
    {
        var order = await unitOfWork.OrderHeader.GetAsync(o => o.Id == vm.OrderHeader.Id, tracked: true);
        if (order is null || order.OrderStatus != SD.StatusProcessing) return BadRequest();

        order.Carrier = vm.OrderHeader.Carrier;
        order.TrackingNumber = vm.OrderHeader.TrackingNumber;
        await unitOfWork.OrderHeader.UpdateStatusAsync(order.Id, SD.StatusShipped);  // 同時寫入出貨日期
        await unitOfWork.SaveAsync();
        TempData[SD.Success] = "訂單已出貨";
        return RedirectToAction(nameof(Details), new { id = order.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await unitOfWork.OrderHeader.GetAsync(o => o.Id == id);
        if (order is null || order.OrderStatus is not (SD.StatusPending or SD.StatusProcessing)) return BadRequest();

        await using var transaction = await unitOfWork.BeginTransactionAsync();
        foreach (var d in await unitOfWork.OrderDetail.GetAllAsync(d => d.OrderHeaderId == id))
            await unitOfWork.Product.IncreaseStockAsync(d.ProductId, d.Count);   // 還原庫存
        await unitOfWork.OrderHeader.UpdateStatusAsync(id, SD.StatusCancelled);
        await unitOfWork.SaveAsync();
        await transaction.CommitAsync();

        TempData[SD.Success] = "訂單已取消，庫存已還原";
        return RedirectToAction(nameof(Details), new { id });
    }

    #region API
    [HttpGet]
    public async Task<IActionResult> GetAll(string? status)
    {
        var orders = await unitOfWork.OrderHeader.GetAllAsync(
            filter: o => string.IsNullOrEmpty(status) || o.OrderStatus == status,
            orderBy: q => q.OrderByDescending(o => o.OrderDate),
            includeProperties: "ApplicationUser");

        var data = orders.Select(o => new
        {
            o.Id, o.Name, o.PhoneNumber, Email = o.ApplicationUser?.Email,
            OrderDate = o.OrderDate.ToString("yyyy/MM/dd HH:mm"),
            Total = o.OrderTotal + o.ShippingFee,
            Status = SD.GetStatusText(o.OrderStatus),
            Badge = SD.GetStatusBadge(o.OrderStatus)
        });
        return Json(new { data });
    }
    #endregion
}
