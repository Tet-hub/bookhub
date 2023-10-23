using Microsoft.AspNetCore.Mvc;

namespace NavOS.Basecode.AdminApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
