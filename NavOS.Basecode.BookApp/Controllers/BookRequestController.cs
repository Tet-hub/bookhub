using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NavOS.Basecode.BookApp.Mvc;
using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using NavOS.Basecode.Services.Services;

namespace NavOS.Basecode.BookApp.Controllers
{
    public class BookRequestsController : ControllerBase<BookRequestsController>
    {
        private readonly IBookRequestService _bookRequestsService;

        public BookRequestsController(IBookService bookService,
                              IBookRequestService bookRequestsService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookRequestsService = bookRequestsService;

        }
        //[HttpGet]
        //public IActionResult RecievedRequest()
        //{
        //    var data = _bookRequestsService.GetBooksRequest();
        //    return View(data);
        //}

        // request a book for user side
        [HttpGet]
        public IActionResult SendRequest()
        {
            //var genres = _genreService.GetGenres();
            //var data = new BookViewModel
            //{
            //    Genres = genres
            //};
            return View();
        }
        /// <summary>
        /// Adds the book.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendRequest(BookRequestViewModel book)
        {
            //var isExist = _bookRequestsService.Validate(book.BookTitle);
            //var genres = _genreService.GetGenres();
            //if (isExist)
            //{
            //    book.Genres = genres;
            //    ModelState.AddModelError("BookTitle", "request already exists");
            //    return View(book);
            //}

            _bookRequestsService.SendRequest(book);
            TempData["SuccessMessage"] = "Request has been sent, wait patiently for 48 hours, once book is approved it will be published in BookHub";
            return RedirectToAction("Index", "Book");
        }
    }
}
