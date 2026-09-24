using EShop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Admin.Controllers;

// 後台營運總覽：統計改由資料庫計算
[Area("Admin")]
public class DashboardController(IUnitOfWork unitOfWork) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await unitOfWork.Category.GetAllAsync();
        ViewData["CategoryCount"] = categories.Count;
        return View(await unitOfWork.Product.GetCategorySummariesAsync());
    }
}
