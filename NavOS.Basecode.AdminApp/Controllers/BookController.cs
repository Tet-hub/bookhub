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
            TempData["ErrorMessage"] = "No Book Found";
            return RedirectToAction("Index");
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
