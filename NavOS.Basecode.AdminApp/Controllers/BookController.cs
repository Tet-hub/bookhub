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
using System.Net;

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
            var data = _bookService.GetBooksWithReviews();
            return View(data);
        }
        /// <summary>
        /// Creates new books.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult NewBooks(string searchQuery = null, string filter = null, string sort = null)
        {
            var data = _bookService.FilterAndSortNewBooks(searchQuery, filter, sort);

            return View(data);
        }
        /// <summary>
        /// TopBooks
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TopBooks(string searchQuery = null, string filter = null, string sort = null)
        {
            var data = _bookService.FilterAndSortTopBooks(searchQuery, filter, sort);
            return View(data);
        }
        /// <summary>
        /// BookDetails
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        public IActionResult BookDetails(string bookId)
        {
            var data = _bookService.GetBookWithReviews(bookId);

            if (data != null)
            {
                return View(data);
            }

            return NotFound();
        }
        /// <summary>
        /// List of Books.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult BookList(string searchQuery = null, string filter = null, string sort = null)
        {
            var data = _bookService.FilterAndSortBooks(searchQuery, filter, sort);
            string[] headers = new string[] { "Book Profile", "Title", "Author", "Reviews", "Actions" };

            ViewBag.headers = headers;
            return View(data);
        }
        /// <summary>
        /// Display the list of Genre in Add Book
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddBook()
        {
            var genres = _genreService.GetGenres();
            var data = new BookViewModel
            {
                Genres = genres
            };

            return View(data);
        }
        /// <summary>
        /// Adds the book.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddBook(BookViewModel book)
        {
            var isExist = _bookService.Validate(book.BookTitle);
            var genres = _genreService.GetGenres();
            if (isExist)
            {
                book.Genres = genres;
                ModelState.AddModelError("BookTitle", "Title already exists");
                return View(book);
            }

            _bookService.AddBook(book, this.UserName);
            TempData["SuccessMessage"] = "Book Added Successfully";
            return RedirectToAction("BookList");
        }

        /// <summary>
        /// Edits the book.
        /// </summary>
        /// <param name="BookId">The book identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditBook(string BookId)
        {
            var data = _bookService.GetBook(BookId);
            if (data == null)
            {
                TempData["ErrorMessage"] = "No Book Found";
                return RedirectToAction("BookList");
            }
            //re-process the data when validation is executed
            data.Genres = !ModelState.IsValid ? _genreService.GetGenres() : data.Genres;

            return View(data);
        }

        /// <summary>
        /// Edits the book.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditBook(BookViewModel book)
        {
            var isExist = _bookService.ValidateForEdit(book.BookTitle, book.BookId);
            var genres = _genreService.GetGenres();
            if (isExist)
            {

                book.Genres = genres;
                TempData["ErrorMessage"] = "Book Title already existed!";
                return RedirectToAction("EditBook", "Book", new { bookId = book.BookId });
            }

            _bookService.UpdateBook(book, this.UserName);
            TempData["SuccessMessage"] = "Book Updated Successfully";
            return RedirectToAction("BookList");
        }

        /// <summary>
        /// Deletes the book.
        /// </summary>
        /// <param name="bookId">The book identifier.</param>
        /// <returns></returns>
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
