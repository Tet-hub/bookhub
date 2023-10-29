using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NavOS.Basecode.AdminApp.Mvc;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;

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
            //Before added, invoke the validation declared in the BookService Validate()
            var isExist = _genreService.Validate(genre.GenreName);

            if (isExist)
            {
                //create a condition to avoid duplication
                //AddModelError needs a "key" and "error message", key is the container of the message error
                //in this part is "Title", then error message viewed by the user
                ModelState.AddModelError("Title", "Title already exist.");
                //here, below why book is returned,
                //in order to avoid wiping out the data of other fields inputted along from the user
                return View(genre);
            }
            //_genreService.AddGenre(genre, this._session.GetString("ÄdminName"));
            _genreService.AddGenre(genre, this.UserName);
            return RedirectToAction("ViewGenre");
        }
    }
}
