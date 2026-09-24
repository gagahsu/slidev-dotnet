using EShop.DataAccess.Repository.IRepository;
using EShop.Models.ViewModels;
using EShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = SD.Policy_Staff)]
public class ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment env) : Controller
{
    public IActionResult Index() => View();

    public async Task<IActionResult> Upsert(int? id)
    {
        ProductVM vm = new() { CategoryList = await GetCategoryListAsync() };

        if (id is null or 0)
            return View(vm);                                     // 新增

        var product = await unitOfWork.Product.GetAsync(p => p.Id == id);
        if (product is null) return NotFound();

        vm.Product = product;                                    // 編輯
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(ProductVM vm, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            vm.CategoryList = await GetCategoryListAsync();
            return View(vm);
        }

        if (file is not null)
            vm.Product.ImageUrl = await SaveImageAsync(file, vm.Product.ImageUrl);

        var isNew = vm.Product.Id == 0;
        if (isNew) unitOfWork.Product.Add(vm.Product);
        else unitOfWork.Product.Update(vm.Product);

        await unitOfWork.SaveAsync();
        TempData[SD.Success] = isNew ? "商品新增成功" : "商品更新成功";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<SelectListItem>> GetCategoryListAsync()
        => (await unitOfWork.Category.GetAllAsync(orderBy: q => q.OrderBy(c => c.DisplayOrder)))
            .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

    private async Task<string> SaveImageAsync(IFormFile file, string? oldImageUrl)
    {
        var folder = Path.Combine(env.WebRootPath, "images", "product");
        Directory.CreateDirectory(folder);                          // 資料夾不存在就建立

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        await using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        DeleteImage(oldImageUrl);                                   // 刪除舊圖片
        return $"/images/product/{fileName}";
    }

    private void DeleteImage(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;
        var path = Path.Combine(env.WebRootPath, imageUrl.TrimStart('/'));
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
    }

    #region API
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await unitOfWork.Product.GetAllAsync(includeProperties: "Category");
        var data = products.Select(p => new
        {
            p.Id, p.Name, p.Origin, p.Price, p.Stock,
            Category = p.Category?.Name
        });
        return Json(new { data });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int? id)
    {
        var product = await unitOfWork.Product.GetAsync(p => p.Id == id);
        if (product is null) return Json(new { success = false, message = "找不到商品" });

        DeleteImage(product.ImageUrl);
        unitOfWork.Product.Remove(product);
        await unitOfWork.SaveAsync();
        return Json(new { success = true, message = "商品刪除成功" });
    }
    #endregion
}
