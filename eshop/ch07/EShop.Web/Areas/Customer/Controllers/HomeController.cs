using System.Diagnostics;
using EShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
