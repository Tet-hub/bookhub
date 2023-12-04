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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        /// List of Admin.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <returns></returns>
        public IActionResult AdminList(string searchQuery)
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Book");
            }
            var data = _adminService.GetAllAdminWithSearch(searchQuery);
            return View(data);
        }

        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public IActionResult AddAdmin()
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Book");
            }
            return View();
        }

        /// <summary>
        /// Adds the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddAdmin(AdminViewModel model)
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Book");
            }

            try
            {
                var _isEmailValid = await _adminService.CheckEmailValidAsync(model.AdminEmail);
                if (_isEmailValid)
                {
                    _adminService.AddAdmin(model, this.UserName);
                    TempData["SuccessMessage"] = "Admin added successfully.";
                    return RedirectToAction("List");
                }
                TempData["ErrorMessage"] = "Invalid Email";
                return View(model);
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }

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
                return RedirectToAction("Index", "Book");
            }
            bool _isAdminDeleted = _adminService.DeleteAdmin(adminId);
            if (_isAdminDeleted)
            {
                TempData["SuccessMessage"] = "Admin deleted successfully.";
                return RedirectToAction("AdminList");
            }
			TempData["ErrorMessage"] = "No Admin was deleted.";
			return RedirectToAction("AdminList");
		}

        /// <summary>
        /// Edits the specified admin identifier.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditAdmin(string adminId) 
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Book");
            }
            var admin = _adminService.GetAdmin(adminId);
            if (admin != null)
            {
                return View(admin);
			}
			TempData["ErrorMessage"] = "Admin not found.";
			return RedirectToAction("AdminList");

		}

        /// <summary>
        /// Edits the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditAdmin(AdminViewModel model) 
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Book");
            }

            var isEmailValid = await _adminService.CheckEmailValidAsync(model.AdminEmail);
            if (isEmailValid)
            {
                bool isEmailExisted = _adminService.CheckEmailExist(model);
                if (!isEmailExisted)
                {
                    bool _isAdminUpdated = _adminService.EditAdmin(model, this.UserName);
                    if (_isAdminUpdated)
                    {
                        TempData["SuccessMessage"] = "Admin updated successfully.";
                        return RedirectToAction("AdminList");
                    }
                    TempData["ErrorMessage"] = "No Admin was updated.";
                    return RedirectToAction("AdminList");
                }
                TempData["ErrorMessage"] = "Admin Email already existed!";
                return RedirectToAction("EditAdmin", "Admin", new { adminId = model.AdminId });
            }
            TempData["ErrorMessage"] = "Invalid Email!";
            return RedirectToAction("EditAdmin", "Admin", new { adminId = model.AdminId });
        }

        [HttpGet]
        public IActionResult AdminSetting()
        {
            string adminId = this._session.GetString("AdminId");
            var data = _adminService.GetAdmin(adminId);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditAdminInfo(AdminViewModel model)
        {
            var isEmailValid = await _adminService.CheckEmailValidAsync(model.AdminEmail);
            if (isEmailValid)
            {
                bool isEmailExisted = _adminService.CheckEmailExist(model);
                if (!isEmailExisted)
                {
                    bool _isAdminUpdated = _adminService.EditAdmin(model, this.UserName);
                    if (_isAdminUpdated)
                    {
                        TempData["SuccessMessage"] = "Admin updated successfully.";
                        return RedirectToAction("AdminSetting");
                    }
                    TempData["ErrorMessage"] = "No Admin was updated.";
                    return RedirectToAction("AdminSetting");
                }
                TempData["ErrorMessage"] = "Admin Email already existed!";
                return RedirectToAction("AdminSetting");
            }
            TempData["ErrorMessage"] = "Invalid Email!";
            return RedirectToAction("AdminSetting");
        }

    }
}
