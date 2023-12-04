using NavOS.Basecode.Data;
using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Repositories;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using NavOS.Basecode.Services.Services;
using NavOS.Basecode.BookApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NavOS.Basecode.Services.Utilities;

namespace NavOS.Basecode.BookApp
{
    // Other services configuration
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Configures the other services.
        /// </summary>
        private void ConfigureOtherServices()
        {
            // Framework
            this._services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            this._services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Common
            this._services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            this._services.AddScoped<IBookService, BookService>();
            this._services.AddScoped<IReviewService, ReviewService>();
            this._services.AddScoped<IGenreService, GenreService>();
            this._services.AddScoped<IEmailSender, EmailSender>();
            this._services.AddScoped<IEmailChecker, EmailChecker>();
          

            // Repositories
            this._services.AddScoped<IBookRepository, BookRepository>();
            this._services.AddScoped<IReviewRepository, ReviewRepository>();
            this._services.AddScoped<IGenreRepository, GenreRepository>();
         

            this._services.AddHttpClient();
        }
    }
}
