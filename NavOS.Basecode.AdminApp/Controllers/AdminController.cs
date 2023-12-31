﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            var data = _adminService.GetAllAdminWithSearch(searchQuery, this.UserId);
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
                    _adminService.AddAdmin(model, Admin());
                    TempData["SuccessMessage"] = "Admin added successfully.";
                    return RedirectToAction("AdminList");
                }
                TempData["ErrorMessage"] = "Invalid Email!";
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
        /// Edits the specified admin.
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
        /// Edits the specified admin.
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
                    bool _isAdminUpdated = _adminService.EditAdmin(model, Admin());
                    if (_isAdminUpdated)
                    {
                        TempData["SuccessMessage"] = "Admin updated successfully.";
                        return RedirectToAction("AdminList");
                    }
                    TempData["ErrorMessage"] = "No Admin was updated.";
                    return RedirectToAction("AdminList");
                }
                TempData["ErrorMessage"] = "Email already existed!";
                return RedirectToAction("EditAdmin", "Admin", new { adminId = model.AdminId });
            }
            TempData["ErrorMessage"] = "Invalid Email!";
            return RedirectToAction("EditAdmin", "Admin", new { adminId = model.AdminId });
        }

        #region Settings
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
                    bool _isAdminUpdated = _adminService.EditAdmin(model, model.AdminName);
                    if (_isAdminUpdated)
                    {
                        TempData["SuccessMessage"] = "Admin updated successfully.";
                        return RedirectToAction("AdminSetting");
                    }
                    TempData["ErrorMessage"] = "No Admin was updated.";
                    return RedirectToAction("AdminSetting");
                }
                TempData["ErrorMessage"] = "Email already existed!";
                return RedirectToAction("AdminSetting");
            }
            TempData["ErrorMessage"] = "Invalid Email!";
            return RedirectToAction("AdminSetting");
        }

        [HttpPost]
        public IActionResult AdminChangePassword(AdminViewModel model)
        {
            var isPasswordCorrect = _adminService.CheckCurrentPassword(model);
            if (isPasswordCorrect)
            {
                var newPassword = _adminService.NewPassword(model);
                if (newPassword)
                {
                    TempData["SuccessMessage"] = "Successfully changed password.";
                    return RedirectToAction("SignOutUser", "Account");
                }
                TempData["ErrorMessage"] = "Error changing password.";
                return Redirect($"{Url.Action("AdminSetting")}#admin-change-password");
            }
            TempData["ErrorMessage"] = "Incorrect Password";
            return Redirect($"{Url.Action("AdminSetting")}#admin-change-password");
        }
        #endregion

        #region Private Method
        private string Admin()
        {
            var admin = _adminService.GetAdmin(this.UserId);
            return admin.AdminName;
        } 
        #endregion
    }
}