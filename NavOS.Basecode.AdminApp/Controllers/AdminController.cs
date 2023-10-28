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

namespace NavOS.Basecode.AdminApp.Controllers
{
    public class AdminController : ControllerBase<AdminController>
    {
        private readonly IAdminService _adminService;
		private readonly SessionManager _sessionManager;
		public AdminController(IAdminService adminService,
                               IHttpContextAccessor httpContextAccessor, 
                               ILoggerFactory loggerFactory, 
                               IConfiguration configuration, 
                               IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
			this._sessionManager = new SessionManager(this._session);
			_adminService = adminService;
        }

         
        public IActionResult List()
        {
			if (this._session.GetString("Role") != "Master Admin")
			{
				return RedirectToAction("Index", "Home");
			}
			var data = _adminService.GetAllAdmins();
            return View(data);
        }

        public IActionResult Add()
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
		public IActionResult Add(AdminViewModel model)
		{
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            _adminService.AddAdmin(model);
            return RedirectToAction("List");
		}

        [HttpPost]
        public IActionResult Delete(AdminViewModel model)
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            bool _isAdminDeleted = _adminService.DeleteAdmin(model);
            if (_isAdminDeleted)
            {
                return RedirectToAction("List");
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Edit(string AdminId) 
        {
            if (this._session.GetString("Role") != "Master Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var admin = _adminService.GetAdmin(AdminId);
            if (admin != null)
            {
                AdminViewModel adminViewModel = new AdminViewModel();
                adminViewModel.AdminId = AdminId;
                adminViewModel.AdminName = admin.AdminName;
                adminViewModel.AdminEmail = admin.AdminEmail;
                adminViewModel.ContactNo = admin.ContactNo;
                adminViewModel.Dob = admin.Dob;
                adminViewModel.Role = admin.Role;

				return View(adminViewModel);
			}
            return NotFound();
            
        }

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
				return RedirectToAction("List");
			}
            return NotFound();
        }
	}
}
