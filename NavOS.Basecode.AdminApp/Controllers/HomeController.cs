using NavOS.Basecode.AdminApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NavOS.Basecode.AdminApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : ControllerBase<HomeController>
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="HomeController"/> class.
		/// </summary>
		/// <param name="httpContextAccessor">HTTP context accessor</param>
		/// <param name="loggerFactory">Logger factory</param>
		/// <param name="configuration">Configuration</param>
		/// <param name="mapper">Mapper</param>
		public HomeController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {

        }

        /// <summary>
        /// Returns Home View.
        /// </summary>
        /// <returns> Home View </returns>
        public IActionResult Index()
        {
			if (this._session.GetString("HasSession") != "Exist")
			{
				return RedirectToAction("Login", "Account");
			}
			return View();
        }
    }
}
