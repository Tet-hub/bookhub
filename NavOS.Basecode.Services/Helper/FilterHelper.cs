using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace NavOS.Basecode.Services.Helper
{
    public static class FilterHelper
    {
        public static string GetQueryString(int page, HttpContext context)
        {
            var query = context.Request.Query;
            var queryParameters = new List<string>();

            if (query.TryGetValue("filter", out var filter))
                queryParameters.Add($"filter={filter}");

            if (query.TryGetValue("sort", out var sort))
                queryParameters.Add($"sort={sort}");

            if (query.TryGetValue("searchQuery", out var searchQuery))
                queryParameters.Add($"searchQuery={searchQuery}");

            queryParameters.Add($"page={page}");

            return "?" + string.Join("&", queryParameters);
        }
    }
}
