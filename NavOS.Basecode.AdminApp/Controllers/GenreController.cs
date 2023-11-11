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
using System.Linq;

namespace NavOS.Basecode.AdminApp.Controllers
{
    public class GenreController : ControllerBase<GenreController>
    {
        
        private readonly IGenreService _genreService;
        private readonly IBookService _bookService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public GenreController(IGenreService genreService,
                              IBookService bookService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            
            _genreService = genreService;
            _bookService = bookService;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ViewGenre()
        {
            var data = _genreService.GetGenres();
            return View(data);
        }
        [HttpGet]
        public IActionResult AddGenre()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddGenre(GenreViewModel genre)
        {
            bool isExist = _genreService.Validate(genre.GenreName);

            if (isExist)
            {
                TempData["ErrorMessage"] = "Title already exist.";
                return View(genre);
            }
            _genreService.AddGenre(genre, this.UserName);
            return RedirectToAction("ViewGenre");
        }

        [HttpGet] 

        public IActionResult EditGenre(string Genreid)
        {

            var genre = _genreService.GetGenre(Genreid); 
            if (genre != null)
            {
                GenreViewModel genreViewModel = new()
                {
                    GenreId = Genreid,
                    GenreName = genre.GenreName,
                    GenreDescription = genre.GenreDescription,
                    
                };

                
                return View(genreViewModel);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult EditGenre(GenreViewModel genreViewModel)
        {
            bool isUpdated = _genreService.UpdateGenre(genreViewModel, this.UserName);
            if (isUpdated)
            {
                TempData["SuccessMessage"] = "Genre Successfully Edited";
                return RedirectToAction("ViewGenre");

            }
            return NotFound();
        }

       


        [HttpGet]
        public IActionResult Delete(string genreId)
        {
            bool isDeleted = _genreService.DeleteGenre(genreId);
            if (isDeleted)
            {
                TempData["SuccessMessage"] = "Genre Successfully Deleted";
                return RedirectToAction("ViewGenre");
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult BooksGenre(string genreName)
        {

            var booksForGenre = _bookService.GetBooksForGenre(genreName); // Replace with your actual method to get books for a genre
            ViewData["GenreName"] = genreName;

            return View(booksForGenre);
        }
        [HttpGet]
        public IActionResult DeleteBook(string bookId)
        {
            bool _isBookDeleted = _bookService.DeleteBook(bookId);
            if (_isBookDeleted)
            {
                TempData["SuccessMessage"] = "Book Deleted Successfully";
                return RedirectToAction("BooksGenre");
            }
            TempData["ErrorMessage"] = "No Book was Deleted";
            return RedirectToAction("BooksGenre");
        }
    }
}
