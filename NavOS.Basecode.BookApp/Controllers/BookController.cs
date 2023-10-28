using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NavOS.Basecode.BookApp.Mvc;
using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Linq;

namespace NavOS.Basecode.BookApp.Controllers
{
    public class BookController : ControllerBase<BookController>
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
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
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookService = bookService;
            _reviewService = reviewService;

        }
        /// <summary>
        /// Homepage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var reviews = _reviewService.GetReviews();
            ViewData["Reviews"] = reviews;

            var book = _bookService.GetBooks();
            ViewData["Books"] = book;

            return View();
        }
        /// <summary>
        /// NewBooks
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult NewBooks(string searchQuery)
        {
            var currentDate = DateTime.Now;
            var twoWeeksAgo = currentDate.AddDays(-14);

            var data = _bookService.GetBooks()
                .Where(book => book.AddedTime >= twoWeeksAgo)
                .OrderByDescending(book => book.AddedTime)
                .ToList();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                data = data.Where(book =>
                    book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                ).ToList();

            }

            return View(data);
        }

        /// <summary>
        /// TopBooks
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TopBooks(string searchQuery)
        {
            var data = _bookService.GetBooks();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                data = data.Where(book =>
                    book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            var reviews = _reviewService.GetReviews();
            ViewData["Reviews"] = reviews;
            ViewData["TopBooks"] = data;

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
                var reviews = _reviewService.GetReviews();

                ViewData["Reviews"] = reviews;
                ViewData["Book"] = book;

                return View();
            }
            return NotFound();
        }

        public IActionResult AddReview(ReviewViewModel review)
        {
            if (string.IsNullOrEmpty(review.ReviewText))
            {
                review.ReviewText = string.Empty;
            }
            _reviewService.AddReview(review);
            return RedirectToAction("BookDetails", new { review.BookId });
        }

    }
}
