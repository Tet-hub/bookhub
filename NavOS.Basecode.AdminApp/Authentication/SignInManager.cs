using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using NavOS.Basecode.AdminApp.Extensions.Configuration;
using NavOS.Basecode.AdminApp.Models;
using NavOS.Basecode.Resources.Constants;
using NavOS.Basecode.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static NavOS.Basecode.Resources.Constants.Enums;

namespace NavOS.Basecode.AdminApp.Authentication
{
    /// <summary>
    /// SignInManager
    /// </summary>
    public class SignInManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public LoginUser user { get; set; }

        /// <summary>
        /// Initializes a new instance of the SignInManager class.
        /// </summary>
        public SignInManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SignInManager class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="accountService">The account service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public SignInManager(IConfiguration configuration,
                             IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
            user = new LoginUser();
        }

        /// <summary>
        /// Gets the claims identity.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The successfully completed task</returns>
        public Task<ClaimsIdentity> GetClaimsIdentity(string username, string password)
        {
            ClaimsIdentity claimsIdentity = null;
            Admin adminData = new Admin();

            user.loginResult = LoginResult.Success;//TODO this._accountService.AuthenticateUser(username, password, ref userData);

            if (user.loginResult == LoginResult.Failed)
            {
                return Task.FromResult<ClaimsIdentity>(null);
            }

            user.adminData = adminData;
            claimsIdentity = CreateClaimsIdentity(adminData);
            return Task.FromResult(claimsIdentity);
        }

        /// <summary>
        /// Creates the claims identity.
        /// </summary>
        /// <param name="admin">The admin.</param>
        /// <returns></returns>
        public ClaimsIdentity CreateClaimsIdentity(Admin admin)
        {
            var token = _configuration.GetTokenAuthentication();
            //TODO
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, admin.AdminId, ClaimValueTypes.String, Const.Issuer),
                new Claim(ClaimTypes.Name, admin.AdminName, ClaimValueTypes.String, Const.Issuer),

                new Claim("AdminId", admin.AdminId, ClaimValueTypes.String, Const.Issuer),
                new Claim("AdminName", admin.AdminName, ClaimValueTypes.String, Const.Issuer),
            };
            return new ClaimsIdentity(claims, Const.AuthenticationScheme);
        }

        /// <summary>
        /// Creates the claims principal.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>Created claims principal</returns>
        public IPrincipal CreateClaimsPrincipal(ClaimsIdentity identity)
        {
            var identities = new List<ClaimsIdentity>();
            identities.Add(identity);
            return this.CreateClaimsPrincipal(identities);
        }

        /// <summary>
        /// Creates the claims principal.
        /// </summary>
        /// <param name="identities">The identities.</param>
        /// <returns>Created claims principal</returns>
        public IPrincipal CreateClaimsPrincipal(IEnumerable<ClaimsIdentity> identities)
        {
            var principal = new ClaimsPrincipal(identities);
            return principal;
        }

        /// <summary>
        /// Signs the in asynchronous.
        /// </summary>
        /// <param name="admin">The admin.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public async Task SignInAsync(Admin admin, bool isPersistent = false)
        {
            var claimsIdentity = this.CreateClaimsIdentity(admin);
            var principal = this.CreateClaimsPrincipal(claimsIdentity);
            await this.SignInAsync(principal, isPersistent);
        }

        /// <summary>
        /// Signs in user asynchronously
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public async Task SignInAsync(IPrincipal principal, bool isPersistent = false)
        {
            var token = _configuration.GetTokenAuthentication();
            await _httpContextAccessor
                .HttpContext
                .SignInAsync(
                            Const.AuthenticationScheme,
                            (ClaimsPrincipal)principal,
                            new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(token.ExpirationMinutes),
                                IsPersistent = isPersistent,
                                AllowRefresh = false
                            });
        }

        /// <summary>
        /// Signs out user asynchronously
        /// </summary>
        public async Task SignOutAsync()
        {
            var token = _configuration.GetTokenAuthentication();
            await _httpContextAccessor.HttpContext.SignOutAsync(Const.AuthenticationScheme);
        }
    }
}
