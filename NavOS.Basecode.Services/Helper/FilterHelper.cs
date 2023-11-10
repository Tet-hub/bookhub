using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Helper
{
    public static class FilterHelper
    {
        public static string GetQueryString(int page, HttpContext context)
        {
            var queryParameters = new List<string>();

            if (context.Request.Query.TryGetValue("filter", out var filter))
                queryParameters.Add($"filter={filter}");

            if (context.Request.Query.TryGetValue("sort", out var sort))
                queryParameters.Add($"sort={sort}");

            if (context.Request.Query.TryGetValue("searchQuery", out var searchQuery))
                queryParameters.Add($"searchQuery={searchQuery}");

            queryParameters.Add($"page={page}");

            return "?" + string.Join("&", queryParameters);
        }
    }

}
