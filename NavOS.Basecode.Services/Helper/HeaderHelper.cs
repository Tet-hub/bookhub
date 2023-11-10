using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Helper
{
    public static class HeaderHelper
    {
        public static (string DisplayText, string TextColor, string FontStyle) GetDisplayProperties(HttpContext context)
        {
            var filter = context.Request.Query["filter"].ToString().ToUpper();
            var searchQuery = context.Request.Query["searchQuery"].ToString();
            var hasFilter = !string.IsNullOrEmpty(filter);
            var hasSearchQuery = !string.IsNullOrEmpty(searchQuery);

            var displayText = hasFilter
                ? $"LIST | {filter} | {searchQuery}"
                : (hasSearchQuery ? $"LIST | {searchQuery}" : "LIST | ");

            var textColor = (hasFilter || hasSearchQuery) ? "inherit" : "#00A651";
            var fontStyle = (hasFilter || hasSearchQuery) ? "normal" : "italic";

            return (displayText, textColor, fontStyle);
        }
    }
}
