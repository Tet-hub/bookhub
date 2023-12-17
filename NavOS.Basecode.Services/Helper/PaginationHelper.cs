using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Helper
{
    /// <summary>
    /// Book Pagination Helper
    /// </summary>
    public static class PaginationHelper
    {
        public static (int, int, int, int, int, List<BookViewModel>) GetPagination(
            int pageSize, List<BookViewModel> books, string currentPageQueryString)
        {
            try
            {
                int currentPage = string.IsNullOrEmpty(currentPageQueryString) ? 1 : int.Parse(currentPageQueryString);
                int totalItems = books.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                int startIndex = (currentPage - 1) * pageSize;
                int bookNumber = startIndex + 1;
                int endIndex = Math.Min(startIndex + pageSize, totalItems);

                // Check if startIndex is out of bounds
                if (startIndex >= totalItems || startIndex < 0)
                {
                    return (-1, -1, -1, -1, -1, new List<BookViewModel>());
                }

                List<BookViewModel> pageData = books.GetRange(startIndex, endIndex - startIndex);
                return (currentPage, totalItems, totalPages, startIndex, bookNumber, pageData);
            }
            catch
            {
                return (-1, -1, -1, -1, -1, new List<BookViewModel>());
            }
        }
    }
    /// <summary>
    /// Review Pagination Helper
    /// </summary>
    public static class ReviewPaginationHelper
    {
        public static (int, int, int, int, int, List<ReviewViewModel>) GetPagination(
            int pageSize, List<ReviewViewModel> reviews, string currentPageQueryString)
        {
            try
            {
                int currentPage = string.IsNullOrEmpty(currentPageQueryString) ? 1 : int.Parse(currentPageQueryString);
                int totalItems = reviews.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                int startIndex = (currentPage - 1) * pageSize;
                int reviewNumber = startIndex + 1;
                int endIndex = Math.Min(startIndex + pageSize, totalItems);

                if (startIndex >= totalItems || startIndex < 0)
                {
                    return (-1, -1, -1, -1, -1, new List<ReviewViewModel>());
                }

                List<ReviewViewModel> pageData = reviews.GetRange(startIndex, endIndex - startIndex);
                return (currentPage, totalItems, totalPages, startIndex, reviewNumber, pageData);
            }
            catch
            {
                return (-1, -1, -1, -1, -1, new List<ReviewViewModel>());
            }
        }
    }

    /// <summary>
    /// Genre Pagination helper
    /// </summary>
    public static class GenrePaginationHelper
    {
        public static (int, int, int, int, int, List<GenreViewModel>) GetPagination(
            int pageSize, List<GenreViewModel> genre, string currentPageQueryString)
        {
            try
            {
                int currentPage = string.IsNullOrEmpty(currentPageQueryString) ? 1 : int.Parse(currentPageQueryString);
                int totalItems = genre.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                int startIndex = (currentPage - 1) * pageSize;
                int bookNumber = startIndex + 1;
                int endIndex = Math.Min(startIndex + pageSize, totalItems);

                if (startIndex >= totalItems || startIndex < 0)
                {
                    return (-1, -1, -1, -1, -1, new List<GenreViewModel>());
                }

                List<GenreViewModel> pageData = genre.GetRange(startIndex, endIndex - startIndex);
                return (currentPage, totalItems, totalPages, startIndex, bookNumber, pageData);
            }
            catch
            {
                return (-1, -1, -1, -1, -1, new List<GenreViewModel>());
            }
        }
    }
    /// <summary>
    /// Admin Pagination Helper
    /// </summary>
    public static class AdminPaginationHelper
    {
        public static (int, int, int, int, int, List<AdminViewModel>) GetPagination(
            int pageSize, List<AdminViewModel> genre, string currentPageQueryString)
        {
            try
            {
                int currentPage = string.IsNullOrEmpty(currentPageQueryString) ? 1 : int.Parse(currentPageQueryString);
                int totalItems = genre.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                int startIndex = (currentPage - 1) * pageSize;
                int bookNumber = startIndex + 1;
                int endIndex = Math.Min(startIndex + pageSize, totalItems);

                if (startIndex >= totalItems || startIndex < 0)
                {
                    return (-1, -1, -1, -1, -1, new List<AdminViewModel>());
                }

                List<AdminViewModel> pageData = genre.GetRange(startIndex, endIndex - startIndex);
                return (currentPage, totalItems, totalPages, startIndex, bookNumber, pageData);
            }
            catch
            {
                return (-1, -1, -1, -1, -1, new List<AdminViewModel>());
            }
        }
    }

}
