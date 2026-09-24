using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using EShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = SD.Policy_Admin)]
public class StoreController(IUnitOfWork unitOfWork) : Controller
{
    public IActionResult Index() => View();

    public async Task<IActionResult> Upsert(int? id)
    {
        if (id is null or 0) return View(new Store());
        var store = await unitOfWork.Store.GetAsync(s => s.Id == id);
        return store is null ? NotFound() : View(store);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(Store store)
    {
        if (!ModelState.IsValid) return View(store);

        var isNew = store.Id == 0;
        if (isNew) unitOfWork.Store.Add(store);
        else unitOfWork.Store.Update(store);

        await unitOfWork.SaveAsync();
        TempData[SD.Success] = isNew ? "分店新增成功" : "分店更新成功";
        return RedirectToAction(nameof(Index));
    }

    #region API
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stores = await unitOfWork.Store.GetAllAsync(orderBy: q => q.OrderBy(s => s.Id));
        return Json(new { data = stores });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int? id)
    {
        var store = await unitOfWork.Store.GetAsync(s => s.Id == id);
        if (store is null) return Json(new { success = false, message = "找不到分店" });

        unitOfWork.Store.Remove(store);
        await unitOfWork.SaveAsync();
        return Json(new { success = true, message = "分店刪除成功" });
    }
    #endregion
}
