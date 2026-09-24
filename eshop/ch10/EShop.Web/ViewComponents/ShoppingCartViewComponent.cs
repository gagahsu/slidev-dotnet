using System.Security.Claims;
using EShop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.ViewComponents;

public class ShoppingCartViewComponent(IUnitOfWork unitOfWork) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        var count = userId is null ? 0
            : (await unitOfWork.ShoppingCart.GetAllAsync(c => c.ApplicationUserId == userId)).Count;
        return View(count);          // Views/Shared/Components/ShoppingCart/Default.cshtml
    }
}
