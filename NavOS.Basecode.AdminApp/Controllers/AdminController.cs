using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NavOS.Basecode.AdminApp.Mvc;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.Manager;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Linq;

namespace NavOS.Basecode.AdminApp.Controllers
{
    public class AdminController : ControllerBase<AdminController>
    {
        private readonly IAdminService _adminService;
        		
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="adminService">The admin service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        public AdminController(IAdminService adminService,
                               IHttpContextAccessor httpContextAccessor, 
                               ILoggerFactory loggerFactory, 
                               IConfiguration configuration, 
                               IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
			_adminService = adminService;
        }

        /// <summary>
        /// Lists the specified search string.
        /// </summary>
        /// <param name="searchString">The search string.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public IActionResult List(string searchQuery, int page = 1, int pageSize = 6)
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var allAdmins = _adminService.GetAllAdmins();

            // Filter by AdminName if searchString is provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                allAdmins = allAdmins.Where(a => a.AdminName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                                 a.AdminEmail.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Paginate the data
            var paginatedAdmins = allAdmins.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(allAdmins.Count / (double)pageSize);
            ViewBag.SearchString = searchQuery; // Pass the searchString to the view

            return View(paginatedAdmins);
        }


        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult Add()
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /// <summary>
        /// Adds the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
		public IActionResult Add(AdminViewModel model)
		{
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            _adminService.AddAdmin(model);
            TempData["SuccessMessage"] = "Admin added successfully.";
            return RedirectToAction("List");
		}

        /// <summary>
        /// Deletes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Delete(string adminId)
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            bool _isAdminDeleted = _adminService.DeleteAdmin(adminId);
            if (_isAdminDeleted)
            {
                TempData["SuccessMessage"] = "Admin deleted successfully.";
                return RedirectToAction("List");
            }
            return NotFound();
        }

        /// <summary>
        /// Edits the specified admin identifier.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(string adminId) 
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var admin = _adminService.GetAdmin(adminId);
            if (admin != null)
            {
                return View(admin);
			}
            return NotFound();
            
        }

        /// <summary>
        /// Edits the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit(AdminViewModel model) 
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            bool _isAdminUpdated = _adminService.EditAdmin(model);
            if (_isAdminUpdated)
            {
                TempData["SuccessMessage"] = "Admin updated successfully.";
                return RedirectToAction("List");
			}
            return NotFound();
        }
	}
}
