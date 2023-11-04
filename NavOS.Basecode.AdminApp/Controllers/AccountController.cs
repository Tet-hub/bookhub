﻿using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.Manager;
using NavOS.Basecode.Services.ServiceModels;
using NavOS.Basecode.AdminApp.Authentication;
using NavOS.Basecode.AdminApp.Models;
using NavOS.Basecode.AdminApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using static NavOS.Basecode.Resources.Constants.Enums;

namespace NavOS.Basecode.AdminApp.Controllers
{
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;
        private readonly IAdminService _adminService;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountController"/> class.
		/// </summary>
		/// <param name="signInManager">The sign in manager.</param>
		/// <param name="httpContextAccessor">The HTTP context accessor.</param>
		/// <param name="loggerFactory">The logger factory.</param>
		/// <param name="configuration">The configuration.</param>
		/// <param name="mapper">The mapper.</param>
		/// <param name="adminService">The admin service.</param>
		/// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
		/// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
		public AccountController(
                            SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IAdminService adminService,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
            this._adminService = adminService;
        }

        /// <summary>
        /// Logins this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (this._session.GetString("HasSession") == "Exist")
            {
                return RedirectToAction("Index", "Book");
            }
            ViewBag.LoginView = true;
			ViewData["Title"] = "Login Page";
			TempData["returnUrl"] = System.Net.WebUtility.UrlDecode(HttpContext.Request.Query["ReturnUrl"]);
            
            return this.View();
        }

		/// <summary>
		/// Logins the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="returnUrl">The return URL.</param>
		/// <returns></returns>
		[HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var imagePath = "https://127.0.0.1:8080";
			Admin admin = null;
            var loginResult = _adminService.AuthenticateAdmin(model.AdminEmail, model.Password, ref admin);
            if (loginResult == LoginResult.Success)
            {
                // 認証OK
                await this._signInManager.SignInAsync(admin);
				this._session.SetString("HasSession", "Exist");
                this._session.SetString("SessionId", Guid.NewGuid().ToString());
                this._session.SetString("AdminId", admin.AdminId);
                this._session.SetString("AdminName", admin.AdminName);
                this._session.SetString("Role", admin.Role);
                this._session.SetString("AdminProfile", Path.Combine(imagePath, admin.AdminId + ".png"));
                TempData["SuccessMessage"] = "Welcome " + admin.AdminName + "!";
                return RedirectToAction("Index", "Book");
            }
            else
            {
                // 認証NG
                TempData["ErrorMessage"] = "Incorrect Email Address or Password";
                return View();
            }
        }

		[HttpGet]
		[AllowAnonymous]
        public ActionResult ForgotPassword()
        {
			ViewData["Title"] = "Forgot Password";
			ViewBag.LoginView = true;
			return View();
        }

        /// <summary>
        /// Sign Out current account and return login view.
        /// </summary>
        /// <returns>Created response view</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            this._sessionManager.Clear();
            await this._signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Bye " + this.UserName + "!";
            return RedirectToAction("Login", "Account");
        }
    }
}
