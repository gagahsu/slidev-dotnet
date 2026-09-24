using System.ComponentModel.DataAnnotations;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using EShop.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Web.Areas.Identity.Pages.Account;

public class RegisterModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    IUnitOfWork unitOfWork) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required, EmailAddress] public string Email { get; set; } = "";

        [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "兩次密碼不一致")]
        [Display(Name = "確認密碼")]
        public string ConfirmPassword { get; set; } = "";

        [Required, Display(Name = "姓名")] public string Name { get; set; } = "";
        [Display(Name = "手機")] public string? PhoneNumber { get; set; }
        [Display(Name = "縣市")] public string? City { get; set; }
        [Display(Name = "地址")] public string? StreetAddress { get; set; }
        [Display(Name = "角色")] public string? Role { get; set; }
        [Display(Name = "所屬分店")] public int? StoreId { get; set; }

        [ValidateNever] public IEnumerable<SelectListItem> RoleList { get; set; } = [];
        [ValidateNever] public IEnumerable<SelectListItem> StoreList { get; set; } = [];
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        await LoadListsAsync();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = Input.Email, Email = Input.Email, Name = Input.Name,
                PhoneNumber = Input.PhoneNumber, City = Input.City, StreetAddress = Input.StreetAddress
            };
            var role = User.IsInRole(SD.Role_Admin) && !string.IsNullOrEmpty(Input.Role)
                ? Input.Role : SD.Role_Customer;                  // 只有 Admin 能指定角色
            if (role == SD.Role_Employee) user.StoreId = Input.StoreId;

            var result = await userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);

                if (!User.IsInRole(SD.Role_Admin))
                    await signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
        }

        await LoadListsAsync();
        return Page();
    }

    private async Task LoadListsAsync()
    {
        Input.RoleList = roleManager.Roles.Select(r => r.Name!)
            .Select(name => new SelectListItem { Text = name, Value = name });
        Input.StoreList = (await unitOfWork.Store.GetAllAsync())
            .Select(s => new SelectListItem { Text = $"{s.City} {s.Name}", Value = s.Id.ToString() });
    }
}
