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
using System;
using System.Linq;

namespace NavOS.Basecode.BookApp.Controllers
{
    public class BookController : ControllerBase<BookController>
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly IGenreService _genreService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public BookController(IBookService bookService,
                              IReviewService reviewService,
                              IGenreService genreService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookService = bookService;
            _reviewService = reviewService;
            _genreService = genreService;

        }
        /// <summary>
        /// Setups the common view data.
        /// </summary>
        private void CommonViewData()
        {
            var genres = _genreService.GetGenres();
            var reviews = _reviewService.GetReviews();
            ViewData["Genre"] = genres;
            ViewData["Reviews"] = reviews;
        }
        /// <summary>
        /// Homepage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var reviews = _reviewService.GetReviews();
            var books = _bookService.GetBooks();

            ViewData["Reviews"] = reviews;
            ViewData["Books"] = books;

            return View();
        }
        /// <summary>
        /// Display new books.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult NewBooks(string searchQuery, string filter, string sort)
        {
            var currentDate = DateTime.Now;
            var twoWeeksAgo = currentDate.AddDays(-14);

            var book = _bookService.FilterAndSortBooksTwoWeeks(searchQuery, filter, sort, twoWeeksAgo, currentDate);
            CommonViewData();
            ViewData["NewBooks"] = book;

            return View();
        }


        /// <summary>
        /// TopBooks
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TopBooks(string searchQuery, string filter, string sort)
        {
            var book = _bookService.FilterAndSortBooks(searchQuery, filter, sort);
            CommonViewData();
            ViewData["TopBooks"] = book;

            return View();
        }
        /// <summary>
        /// Alls the books.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AllBooks(string searchQuery, string filter, string sort)
        {
            var book = _bookService.FilterAndSortBooks(searchQuery, filter, sort);

            CommonViewData();

            ViewData["AllBooks"] = book;

            return View();
        }

        /// <summary>
        /// BookDetails
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        public IActionResult BookDetails(string BookId)
        {
            var book = _bookService.GetBook(BookId);
            if (book != null)
            {
                var reviews = _reviewService.GetReviews(BookId);

                ViewData["Reviews"] = reviews;
                ViewData["Book"] = book;

                return View();
            }
            return NotFound();
        }
    }
}
