using EShop.DataAccess.Catalog;
using EShop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Admin.Controllers;

// 後台營運總覽：同時用到資料庫（UnitOfWork）與商品目錄
[Area("Admin")]
public class DashboardController(IUnitOfWork unitOfWork, IProductCatalog catalog)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await unitOfWork.Category.GetAllAsync();
        ViewData["CategoryCount"] = categories.Count;
        return View(catalog.GetCategorySummaries());
    }
}
