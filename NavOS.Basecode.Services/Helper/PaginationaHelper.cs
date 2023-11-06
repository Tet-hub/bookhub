using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Helper
{
    public static class PaginationHelper
    {
        public static (int, int, int, int, int, List<BookViewModel>) GetPagination(
            int pageSize, List<BookViewModel> books, string currentPageQueryString)
        {
            int currentPage = string.IsNullOrEmpty(currentPageQueryString) ? 1 : int.Parse(currentPageQueryString);
            int totalItems = books.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            int startIndex = (currentPage - 1) * pageSize;
            int bookNumber = startIndex + 1;
            int endIndex = Math.Min(startIndex + pageSize, totalItems);
            List<BookViewModel> pageData = books.GetRange(startIndex, endIndex - startIndex);

            return (currentPage, totalItems, totalPages, startIndex, bookNumber, pageData);
        }
    }
}
