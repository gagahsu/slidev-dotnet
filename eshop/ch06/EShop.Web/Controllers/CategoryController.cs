using EShop.Web.Data;
using EShop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Web.Controllers;

public class CategoryController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await db.Categories
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
        return View(categories);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (category.Name == category.DisplayOrder.ToString())
            ModelState.AddModelError("Name", "分類名稱不能與顯示順序相同");
        await CheckDuplicateNameAsync(category);

        if (!ModelState.IsValid)
            return View(category);

        db.Categories.Add(category);
        await db.SaveChangesAsync();
        TempData["success"] = "分類新增成功";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null or 0) return NotFound();

        var category = await db.Categories.FindAsync(id);
        if (category is null) return NotFound();

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category category)
    {
        await CheckDuplicateNameAsync(category);
        if (!ModelState.IsValid) return View(category);

        db.Categories.Update(category);
        await db.SaveChangesAsync();
        TempData["success"] = "分類更新成功";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null or 0) return NotFound();
        var category = await db.Categories.FindAsync(id);
        if (category is null) return NotFound();
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int? id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null) return NotFound();

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        TempData["success"] = "分類刪除成功";
        return RedirectToAction(nameof(Index));
    }

    // 分類名稱不可重複：新增時比對全部，編輯時排除自己
    private async Task CheckDuplicateNameAsync(Category category)
    {
        category.Name = category.Name.Trim();
        var exists = await db.Categories.AnyAsync(
            c => c.Name == category.Name && c.Id != category.Id);
        if (exists)
        {
            ModelState.AddModelError(nameof(Category.Name), "此分類名稱已存在");
        }
    }
}
