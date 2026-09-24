using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Models.ViewModels;

public class ProductVM
{
    public Product Product { get; set; } = new();

    [ValidateNever]
    public IEnumerable<SelectListItem> CategoryList { get; set; } = [];
}
