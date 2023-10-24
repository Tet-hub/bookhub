using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NavOS.Basecode.BookApp.Mvc;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Linq;

namespace NavOS.Basecode.BookApp.Controllers
{
    public class BookController : ControllerBase<BookController>
    {
        private readonly IBookService _bookService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public BookController(IBookService bookService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookService = bookService;

        }
        [HttpGet]
        public IActionResult Index()
        {
            var data = _bookService.GetBooks();
            return View(data);
        }
        [HttpGet]
        public IActionResult ViewNewBooks()
        {
            var currentDate = DateTime.Now;

            // Calculate the date two weeks ago
            var twoWeeksAgo = currentDate.AddDays(-14);

            // Filter and sort the books
            var data = _bookService.GetBooks()
                // Filter books added in the last two weeks
                .Where(book => book.AddedTime >= twoWeeksAgo)
                // Sort by AddedTime in descending order
                .OrderByDescending(book => book.AddedTime)
                .ToList();

            return View(data);
        }

        [HttpGet]
        public IActionResult ViewTopBooks()
        {
            // Filter and sort the books
            var data = _bookService.GetBooks()
                // Sort by AddedTime in descending order
                .OrderBy(book => book.AddedTime)
                .ToList();
            return View(data);
        }
        public IActionResult ViewSingleBook(string BookId)
        {
            var book = _bookService.GetBook(BookId);
            if (book != null)
            {
                return View(book);
            }
            return NotFound();
        }

    }
}
