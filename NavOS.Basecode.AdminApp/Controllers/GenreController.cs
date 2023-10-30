using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NavOS.Basecode.AdminApp.Mvc;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using NavOS.Basecode.Services.Services;

namespace NavOS.Basecode.AdminApp.Controllers
{
    public class GenreController : ControllerBase<GenreController>
    {
        
        private readonly IGenreService _genreService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public GenreController(IGenreService genreService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            
            _genreService = genreService;

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
            var isExist = _genreService.Validate(genre.GenreName);

            if (isExist)
            {
                ModelState.AddModelError("Title", "Title already exist.");
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
                return RedirectToAction("ViewGenre");
            }
            return NotFound();
        }

       


        [HttpPost]
        public IActionResult Delete(GenreViewModel genreViewModel)
        {
            bool isDeleted = _genreService.DeleteGenre(genreViewModel);
            if (isDeleted)
            {
                return RedirectToAction("ViewGenre");
            }
            return NotFound();
        }


    }
}
