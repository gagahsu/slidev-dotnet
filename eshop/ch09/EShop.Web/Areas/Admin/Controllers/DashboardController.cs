using EShop.DataAccess.Repository.IRepository;
using EShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Admin.Controllers;

// 後台營運總覽：統計改由資料庫計算
[Area("Admin")]
[Authorize(Policy = SD.Policy_Staff)]
public class DashboardController(IUnitOfWork unitOfWork) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await unitOfWork.Category.GetAllAsync();
        ViewData["CategoryCount"] = categories.Count;
        return View(await unitOfWork.Product.GetCategorySummariesAsync());
    }
}
