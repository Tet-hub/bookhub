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

    public static class ReviewPaginationHelper
    {
        public static (int, int, int, int, int, List<ReviewViewModel>) GetPagination(
            int pageSize, List<ReviewViewModel> reviews, string currentPageQueryString)
        {
            int currentPage = string.IsNullOrEmpty(currentPageQueryString) ? 1 : int.Parse(currentPageQueryString);
            int totalItems = reviews.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            int startIndex = (currentPage - 1) * pageSize;
            int reviewNumber = startIndex + 1;
            int endIndex = Math.Min(startIndex + pageSize, totalItems);
            List<ReviewViewModel> pageData = reviews.GetRange(startIndex, endIndex - startIndex);

            return (currentPage, totalItems, totalPages, startIndex, reviewNumber, pageData);
        }
    }
    public static class GenrePaginationHelper
    {
        public static (int, int, int, int, int, List<GenreViewModel>) GetPagination(
            int pageSize, List<GenreViewModel> genre, string currentPageQueryString)
        {
            int currentPage = string.IsNullOrEmpty(currentPageQueryString) ? 1 : int.Parse(currentPageQueryString);
            int totalItems = genre.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            int startIndex = (currentPage - 1) * pageSize;
            int bookNumber = startIndex + 1;
            int endIndex = Math.Min(startIndex + pageSize, totalItems);
            List<GenreViewModel> pageData = genre.GetRange(startIndex, endIndex - startIndex);

            return (currentPage, totalItems, totalPages, startIndex, bookNumber, pageData);
        }
    }

}
