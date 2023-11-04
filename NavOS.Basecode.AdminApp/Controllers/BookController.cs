using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NavOS.Basecode.AdminApp.Mvc;
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
        /// List of Books.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult BookList(string searchQuery, int page = 1, int pageSize = 5)
        {
            var books = _bookService.GetBooks();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                books = books.Where(a => a.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                                 a.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                                 a.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            string[] headers = new string[] { "Book Profile", "Title", "Author", "Reviews", "Actions" };
            var paginatedBooks = books.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(books.Count / (double)pageSize);
            ViewBag.headers = headers;
            ViewBag.SearchString = searchQuery;
            return View(paginatedBooks);
        }

        /// <summary>
        /// Edits the book.
        /// </summary>
        /// <param name="BookId">The book identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditBook(string BookId)
        {
            var book = _bookService.GetBook(BookId);
            if (book == null)
            {
                TempData["ErrorMessage"] = "No Book Found";
                return RedirectToAction("BookList");
            }
            var genre = _genreService.GetGenres();
            ViewData["Genre"] = genre;
            return View(book);
        }

        [HttpPost]
        public IActionResult EditBook(BookViewModel book)
        {
            if (book.SelectedGenres != null && book.SelectedGenres.Count > 0)
            {
                book.Genre = string.Join(", ", book.SelectedGenres);
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    var genres = _genreService.GetGenres();
                    ViewData["Genre"] = genres;
                }
            }
            _bookService.UpdateBook(book, this.UserName);
            TempData["SuccessMessage"] = "Book Updated Successfully";
            return RedirectToAction("BookList");

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


            return View();
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
                    book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
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
            TempData["ErrorMessage"] = "No Book Found";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Adds the review.
        /// </summary>
        /// <param name="review">The review.</param>
        /// <returns></returns>
        public IActionResult AddReview(ReviewViewModel review)
        {
            if (string.IsNullOrEmpty(review.ReviewText))
            {
                review.ReviewText = string.Empty;
            }
            _reviewService.AddReview(review);
            return RedirectToAction("BookDetails", new { review.BookId });
        }
        
        [HttpGet]
        public IActionResult AddBook()
        {
            var genre = _genreService.GetGenres();
            ViewData["Genre"] = genre;
            return View();
        }
        
        [HttpPost]
        public IActionResult AddBook(BookViewModel book)
        {
            var isExist = _bookService.Validate(book.BookTitle);
            if (isExist)
            {
                if (!ModelState.IsValid)
                {
                    var genres = _genreService.GetGenres();
                    ViewData["Genre"] = genres;
                }
                ModelState.AddModelError("BookTitle", "Title already exists");
                return View();
            }
            if (book.SelectedGenres != null && book.SelectedGenres.Count > 0)
            {
                book.Genre = string.Join(", ", book.SelectedGenres);
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    var genres = _genreService.GetGenres();
                    ViewData["Genre"] = genres;
                }
            }
            _bookService.AddBook(book, this.UserName);
            TempData["SuccessMessage"] = "Book Added Successfully";
            return RedirectToAction("BookList");
        }

        [HttpGet]
        public IActionResult DeleteBook(string bookId)
        {
            bool _isBookDeleted = _bookService.DeleteBook(bookId);
            if (_isBookDeleted)
            {
                TempData["SuccessMessage"] = "Book Deleted Successfully";
                return RedirectToAction("BookList");
            }
            TempData["ErrorMessage"] = "No Book was Deleted";
            return RedirectToAction("BookList");
        }

    }
}
