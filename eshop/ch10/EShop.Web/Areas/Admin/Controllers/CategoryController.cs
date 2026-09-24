using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using EShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = SD.Policy_Admin)]
public class CategoryController(IUnitOfWork unitOfWork) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await unitOfWork.Category.GetAllAsync(
            orderBy: q => q.OrderBy(c => c.DisplayOrder));
        return View(categories);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (category.Name == category.DisplayOrder.ToString())
            ModelState.AddModelError("Name", "分類名稱不能與顯示順序相同");
        await CheckDuplicateNameAsync(category);
        if (!ModelState.IsValid) return View(category);

        unitOfWork.Category.Add(category);
        await unitOfWork.SaveAsync();
        TempData[SD.Success] = "分類新增成功";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null or 0) return NotFound();
        var category = await unitOfWork.Category.GetAsync(c => c.Id == id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category category)
    {
        await CheckDuplicateNameAsync(category);
        if (!ModelState.IsValid) return View(category);

        unitOfWork.Category.Update(category);
        await unitOfWork.SaveAsync();
        TempData[SD.Success] = "分類更新成功";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null or 0) return NotFound();
        var category = await unitOfWork.Category.GetAsync(c => c.Id == id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int? id)
    {
        var category = await unitOfWork.Category.GetAsync(c => c.Id == id);
        if (category is null) return NotFound();
        unitOfWork.Category.Remove(category);
        await unitOfWork.SaveAsync();
        TempData[SD.Success] = "分類刪除成功";
        return RedirectToAction(nameof(Index));
    }

    // 分類名稱不可重複：新增時比對全部，編輯時排除自己
    private async Task CheckDuplicateNameAsync(Category category)
    {
        category.Name = category.Name.Trim();
        if (await unitOfWork.Category.IsNameExistsAsync(category.Name, category.Id))
            ModelState.AddModelError(nameof(Category.Name), "此分類名稱已存在");
    }
}
