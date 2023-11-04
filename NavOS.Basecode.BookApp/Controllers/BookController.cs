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

            var data = _bookService.GetBooks()
                .Where(book => book.AddedTime >= twoWeeksAgo)
                .OrderByDescending(book => book.AddedTime)
                .ToList();

            var reviews = _reviewService.GetReviews();

            if (string.IsNullOrEmpty(filter) || string.Equals(filter, "all", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    data = data
                        .Where(book =>
                            (book.AddedTime >= twoWeeksAgo) && 
                            (book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }
            }
            else if (!string.IsNullOrEmpty(searchQuery))
            {
                switch (filter.ToLower())
                {
                    case "title":
                        data = data
                            .Where(book => book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "author":
                        data = data
                            .Where(book => book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "genre":
                        data = data
                            .Where(book => book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                }
            }
            if (string.Equals(sort, "title", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.BookTitle, StringComparer.OrdinalIgnoreCase).ToList();
            }
            else if (string.Equals(sort, "ratings", StringComparison.OrdinalIgnoreCase))
            {
                var bookAverages = reviews
                    .GroupBy(r => r.BookId)
                    .ToDictionary(g => g.Key, g => g.Average(r => (double?)r.Rate) ?? 0.0);

                data = data.OrderByDescending(book => bookAverages.ContainsKey(book.BookId) ? bookAverages[book.BookId] : 0.0).ToList();
            }
            else if (string.Equals(sort, "author", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.Author, StringComparer.OrdinalIgnoreCase).ToList();
            }

            ViewData["Books"] = data;
            ViewData["Reviews"] = reviews;

            var genres = _genreService.GetGenres();
            ViewData["Genre"] = genres;


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
            var data = _bookService.GetBooks();
            var reviews = _reviewService.GetReviews();

            if (string.IsNullOrEmpty(filter) || string.Equals(filter, "all", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    data = data
                        .Where(book =>
                            (book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }
            }
            else if (!string.IsNullOrEmpty(searchQuery))
            {
                switch (filter.ToLower())
                {
                    case "title":
                        data = data
                            .Where(book => book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "author":
                        data = data
                            .Where(book => book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "genre":
                        data = data
                            .Where(book => book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                }
            }
            if (string.Equals(sort, "title", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.BookTitle, StringComparer.OrdinalIgnoreCase).ToList();
            }
            else if (string.Equals(sort, "author", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.Author, StringComparer.OrdinalIgnoreCase).ToList();
            }

            ViewData["Reviews"] = reviews;
            ViewData["TopBooks"] = data;

            var genres = _genreService.GetGenres();
            ViewData["Genre"] = genres;


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
        public IActionResult AllBooks(string searchQuery, string sort)
        {
            var data = _bookService.GetBooks();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                data = data.Where(book =>
                    book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            if (string.Equals(sort, "title", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.BookTitle, StringComparer.OrdinalIgnoreCase).ToList();
            }
            else if (string.Equals(sort, "author", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.Author, StringComparer.OrdinalIgnoreCase).ToList();
            }
            var reviews = _reviewService.GetReviews();
            var genres = _genreService.GetGenres();
            ViewData["Genre"] = genres;
            ViewData["Reviews"] = reviews;
            ViewData["AllBooks"] = data;

            return View();
        }

    }
}
