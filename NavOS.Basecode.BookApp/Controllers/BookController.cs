using Microsoft.AspNetCore.Mvc;

namespace NavOS.Basecode.BookApp.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            return View("ViewBooks");
        }
        public IActionResult ViewBooks()
        {
            return View("ViewSingleBook");
        }
    }
}
