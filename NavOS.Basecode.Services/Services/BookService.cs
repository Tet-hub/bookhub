using Microsoft.Extensions.Options;
using NavOS.Basecode.Data;
using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Data.Repositories;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace NavOS.Basecode.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreService _genreService;
        private readonly IReviewService _reviewService;
        public BookService(IBookRepository bookRepository,
            IGenreService genreService,
            IReviewService reviewService)
        {
            _bookRepository = bookRepository;
            _genreService = genreService;
            _reviewService = reviewService;
        }

        /// <summary>
        /// Gets the books.
        /// </summary>
        /// <returns></returns>
        public List<BookViewModel> GetBooks()
        {
            var url = "https://127.0.0.1:8080/";
            var data = _bookRepository.GetBooks().Select(s => new BookViewModel
            {
                BookId = s.BookId,
                BookTitle = s.BookTitle,
                Summary = s.Summary,
                Author = s.Author,
                Status = s.Status,
                Genre = s.Genre,
                Pages = s.Pages,
                DateReleased = s.DateReleased,
                AddedTime = s.AddedTime,
                Source = s.Source,
                ImageUrl = Path.Combine(url, s.BookId + ".png"),
                ReviewCount = s.Reviews.Count,
                AvgRatings = s.Reviews.Any() ? Math.Round((double)s.Reviews.Sum(r => r.Rate) / s.Reviews.Count, 1) : 0,
        })
            .ToList();

            return data;
        }

        /// <summary>
        /// Gets the books with reviews -> used for index.cstml
        /// </summary>
        /// <returns></returns>
        public BookWithReviewViewModel GetBooksWithReviews()
        {
            var url = PathManager.BaseURL;
            //var url = "https://127.0.0.1:8080/";

            var booksData = _bookRepository.GetBooks()
                .Select(s => new BookViewModel
                {
                    BookId = s.BookId,
                    BookTitle = s.BookTitle,
                    Summary = s.Summary,
                    Author = s.Author,
                    Status = s.Status,
                    Genre = s.Genre,
                    Pages = s.Pages,
                    DateReleased = s.DateReleased,
                    AddedTime = s.AddedTime,
                    Source = s.Source,
                    ImageUrl = Path.Combine(url, s.BookId + ".png"),
                    ReviewCount = s.Reviews.Count,
                    AvgRatings = s.Reviews.Any() ? Math.Round((double)s.Reviews.Sum(r => r.Rate) / s.Reviews.Count, 1) : 0,
                })
                .ToList();

            var reviewsData = _reviewService.GetReviews()
                .GroupBy(r => r.BookId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var bookWithReviews = new BookWithReviewViewModel
            {
                Books = booksData,
                Reviews = new List<ReviewViewModel>()
            };

            if (bookWithReviews != null)
            {
                foreach (var book in booksData)
            {
                if (reviewsData.TryGetValue(book.BookId, out var bookReviews))
                {
                    book.Reviews = bookReviews.Select(r => new ReviewViewModel
                    {
                    }).ToList();
                }
                else
                {
                    book.Reviews = new List<ReviewViewModel>();
                }
            }
            var latestBooks = bookWithReviews.Books
                .Where(book => (DateTime.Now - book.AddedTime).TotalDays <= 14)
                .OrderByDescending(book => book.AddedTime)
                .Take(5)
                .ToList();

            var topRatedBooks = bookWithReviews.Books
                .Where(book => book.AvgRatings >= 3 && book.ReviewCount > 1)
                .OrderByDescending(book => book.ReviewCount)
                .ThenByDescending(book => book.AvgRatings)
                .Take(5)
                .ToList();
            var topReviewedBooks = bookWithReviews.Books
                .Where(book => book.ReviewCount > 0)
                .OrderByDescending(book => book.ReviewCount)
                .ThenByDescending(book => book.AvgRatings)
                .Take(10)
                .ToList();
                
            bookWithReviews.Books = topReviewedBooks;
            bookWithReviews.LatestBooks = latestBooks;
            bookWithReviews.TopRatedBooks = topRatedBooks;

            return bookWithReviews;
            }

            return null;
        }

        /// <summary>
        /// Gets the book.
        /// </summary>
        /// <param name="BookId">The book identifier.</param>
        /// <returns></returns>
        public BookViewModel GetBook(string BookId)
        {
            var url = PathManager.BaseURL;
            //var url = "https://127.0.0.1:8080/";
            var book = _bookRepository.GetBooks().FirstOrDefault(s => s.BookId == BookId);
            var genres = _genreService.GetGenres();
            var bookViewModel = new BookViewModel
            {
                BookId = book.BookId,
                BookTitle = book.BookTitle,
                Summary = book.Summary,
                Author = book.Author,
                Status = book.Status,
                Genre = book.Genre,
                Pages = book.Pages,
                DateReleased = book.DateReleased,
                AddedTime = book.AddedTime,
                Source = book.Source,
                ImageUrl = Path.Combine(url, book.BookId + ".png"),
                Genres = genres

            };
            return bookViewModel;
        }

        /// <summary>
        /// Adds the book.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="user">The user.</param>
        public void AddBook(BookViewModel book, string user)
        {
            var genres = _genreService.GetGenres();

            var coverImagesPath = PathManager.DirectoryPath.CoverImagesDirectory;

            var model = new Book
            {
                BookId = Guid.NewGuid().ToString(),
                BookTitle = book.BookTitle,
                Summary = book.Summary,
                Author = book.Author,
                Status = book.Status,
                Pages = book.Pages,
                DateReleased = book.DateReleased,
                AddedBy = user,
                UpdatedBy = user,
                AddedTime = DateTime.Now,
                Source = book.Source,
                UpdatedTime = DateTime.Now
            };

            if (book.SelectedGenres != null && book.SelectedGenres.Any())
            {
                model.Genre = string.Join(", ", book.SelectedGenres);
            }

            if (book.ImageFile != null)
            {
                var coverImageFileName = Path.Combine(coverImagesPath, model.BookId) + ".png";
                using (var fileStream = new FileStream(coverImageFileName, FileMode.Create))
                {
                    book.ImageFile.CopyTo(fileStream);
                }
            }
            book.Genres = genres;
            _bookRepository.AddBook(model);
        }
        /// <summary>
        /// Deletes the book.
        /// </summary>
        /// <param name="bookId">The book identifier.</param>
        /// <returns></returns>
        public bool DeleteBook(string bookId)
        {
            var coverImagesPath = PathManager.DirectoryPath.CoverImagesDirectory;
            Book book = _bookRepository.GetBooks().FirstOrDefault(x => x.BookId == bookId);
            if (book != null)
            {
                var image = Path.Combine(coverImagesPath, book.BookId) + ".png";
                File.Delete(image);
                _bookRepository.DeleteBook(book);
                return true;
            }
            return false;

        }
        /// <summary>
        /// Updates the book.
        /// </summary>
        /// <param name="bookViewModel">The book view model.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public bool UpdateBook(BookViewModel bookViewModel, string user)
        {
            var coverImagesPath = PathManager.DirectoryPath.CoverImagesDirectory;
            Book book = _bookRepository.GetBooks().Where(x => x.BookId == bookViewModel.BookId).FirstOrDefault();
            var genres = _genreService.GetGenres();

            if (book != null)
            {
                book.BookTitle = bookViewModel.BookTitle;
                book.Summary = bookViewModel.Summary;
                book.Author = bookViewModel.Author;
                book.Status = bookViewModel.Status;
                book.Genre = bookViewModel.Genre;
                book.Pages = bookViewModel.Pages;
                book.DateReleased = bookViewModel.DateReleased;
                book.UpdatedBy = user;
                book.UpdatedTime = DateTime.Now;
                book.Source = bookViewModel.Source;

                if (bookViewModel.SelectedGenres != null && bookViewModel.SelectedGenres.Count > 0)
                {
                    book.Genre = string.Join(", ", bookViewModel.SelectedGenres);
                }

                if (bookViewModel.ImageFile != null)
                {
                    var coverImageFileName = Path.Combine(coverImagesPath, book.BookId) + ".png";
                    using (var fileStream = new FileStream(coverImageFileName, FileMode.Create))
                    {
                        bookViewModel.ImageFile.CopyTo(fileStream);
                    }
                }

                bookViewModel.Genres = genres;

                _bookRepository.UpdateBook(book);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Gets the book with reviews.
        /// </summary>
        /// <param name="bookId">The book identifier.</param>
        /// <returns></returns>
        public BookWithReviewViewModel GetBookWithReviews(string bookId)
        {
            var book = _bookRepository.GetBooks().FirstOrDefault(s => s.BookId == bookId);

            if (book != null)
            {
                var url = PathManager.BaseURL;
                //var url = "https://127.0.0.1:8080/";
                var reviews = _reviewService.GetReviews(bookId).OrderByDescending(r => r.DateReviewed).ToList();

                var data = new BookWithReviewViewModel
                {
                    BookDetails = new BookViewModel
                    {
                        BookId = book.BookId,
                        BookTitle = book.BookTitle,
                        Summary = book.Summary,
                        Author = book.Author,
                        Status = book.Status,
                        Genre = book.Genre,
                        Pages = book.Pages,
                        DateReleased = book.DateReleased,
                        AddedTime = book.AddedTime,
                        Source = book.Source,
                        ImageUrl = Path.Combine(url, book.BookId + ".png"),
                        Reviews = reviews,
                        AverageRate = reviews.Any() ? reviews.Average(r => r.Rate) : 0.0,
                        ReviewCount = reviews.Count
                    },
                    Reviews = reviews,
                    Review = new ReviewViewModel
                    {
                        BookId = book.BookId
                    }
                };

                return data;
            }

            return null;
        }

        #region Filter Books Logic
        /// <summary>
        /// Filters the and sort books. <= used from BookList
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public FilteredBooksViewModel FilterAndSortBooks(string searchQuery = null, string filter = null, string sort = null)
        {
            var result = new FilteredBooksViewModel();
            result.Books = GetBooks();

            var currentDate = DateTime.Now;

            result.Books = result.Books
                .Where(book => book.AddedTime <= currentDate)
                .OrderByDescending(book => book.AddedTime)
                .ToList();


            var options = new BookFilterOptions
            {
                SearchQuery = searchQuery,
                Filter = filter,
                Sort = sort
            };

            ApplyFilter(options, result);
            ApplySort(options, result);

            result.Genres = _genreService.GetGenres();
            return result;
        }
        /// <summary>
        /// Filters the and sort all book list.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public FilteredBooksViewModel FilterAndSortAllBookList(string searchQuery = null, string filter = null, string sort = null)
        {
            var result = new FilteredBooksViewModel();
            result.Books = GetBooks();

            var options = new BookFilterOptions
            {
                SearchQuery = searchQuery,
                Filter = filter,
                Sort = sort
            };

            ApplyFilter(options, result);
            ApplySort(options, result);

            result.Genres = _genreService.GetGenres();
            return result;
        }
        /// <summary>
        /// Filters the and sort top books.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public FilteredBooksViewModel FilterAndSortTopBooks(string searchQuery = null, string filter = null, string sort = null)
        {
            var result = new FilteredBooksViewModel();
            result.Books = GetBooks();

            var options = new BookFilterOptions
            {
                SearchQuery = searchQuery,
                Filter = filter,
                Sort = sort
            };

            result.Books = result.Books
                .Where(book => book.AvgRatings >= 3 && book.ReviewCount > 1)
                .OrderByDescending(book => book.ReviewCount)
                .ThenByDescending(book => book.AvgRatings)
                .ToList();


            ApplyFilter(options, result);
            ApplySort(options, result);

            result.Genres = _genreService.GetGenres();
            return result;
        }
        /// <summary>
        /// Filters the and sort new books.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public FilteredBooksViewModel FilterAndSortNewBooks(string searchQuery = null, string filter = null, string sort = null)
        {
            var result = new FilteredBooksViewModel();
            result.Books = GetBooks();

            var options = new BookFilterOptions
            {
                SearchQuery = searchQuery,
                Filter = filter,
                Sort = sort
            };

            var currentDate = DateTime.Now;
            var twoWeeksAgo = currentDate.AddDays(-14);

            result.Books = result.Books
                .Where(book => book.AddedTime >= twoWeeksAgo && book.AddedTime <= currentDate)
                .ToList();

            ApplyFilter(options, result);
            ApplySort(options, result);

            result.Genres = _genreService.GetGenres();
            return result;
        }
        #endregion

        #region Validate Book titles
        /// <summary>
        /// Validates the specified book title.
        /// </summary>
        /// <param name="BookTitle">The book title.</param>
        /// <returns></returns>
        public bool Validate(string BookTitle)
        {
            var isExist = _bookRepository.GetBooks().Where(x => x.BookTitle == BookTitle).Any();
            return isExist;
        }
        /// <summary>
        /// Validates for edit.
        /// </summary>
        /// <param name="BookTitle">The book title.</param>
        /// <param name="bookId">The book identifier.</param>
        /// <returns></returns>
        public bool ValidateForEdit(string BookTitle, string bookId)
        {
            var isExist = _bookRepository.GetBooks()
                            .Any(x => x.BookTitle == BookTitle && x.BookId != bookId.ToString());

            return isExist;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="result">The result.</param>
        private void ApplyFilter(BookFilterOptions options, FilteredBooksViewModel result)
        {
            if (options == null)
                return;

            // Genre filter
            if (!string.IsNullOrEmpty(options.Filter) && string.Equals(options.Filter.ToLower(), "genre", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(options.Sort))
                {
                    result.Books = result.Books
                        .Where(book => book.Genre.Contains(options.Sort, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Apply title search within the selected genre
                if (!string.IsNullOrEmpty(options.SearchQuery))
                {
                    result.Books = result.Books
                        .Where(book => book.BookTitle.Contains(options.SearchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }
            // All filters
            else if (string.IsNullOrEmpty(options.Filter) || string.Equals(options.Filter, "all", StringComparison.OrdinalIgnoreCase) || string.Equals(options.Filter, "ratings", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(options.SearchQuery))
                {
                    result.Books = result.Books
                        .Where(book =>
                            (book.BookTitle.Contains(options.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Author.Contains(options.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Genre.Contains(options.SearchQuery, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }
            }
            //Filters based on the selected filter
            else if (!string.IsNullOrEmpty(options.SearchQuery))
            {
                switch (options.Filter.ToLower())
                {
                    case "title":
                        result.Books = result.Books
                            .Where(book => book.BookTitle.Contains(options.SearchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "author":
                        result.Books = result.Books
                            .Where(book => book.Author.Contains(options.SearchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                }
            }
        }

        /// <summary>
        /// Applies the sort.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="result">The result.</param>
        private void ApplySort(BookFilterOptions options, FilteredBooksViewModel result)
        {
            if (options == null || string.IsNullOrEmpty(options.Sort))
                return;

            switch (options.Sort.ToLower())
            {
                case "title":
                    result.Books = result.Books.OrderBy(book => book.BookTitle, StringComparer.OrdinalIgnoreCase).ToList();
                    break;
                case "author":
                    result.Books = result.Books.OrderBy(book => book.Author, StringComparer.OrdinalIgnoreCase).ToList();
                    break;
                case "ratings":
                    result.Books = result.Books.OrderByDescending(book => book.AvgRatings).ToList();
                    break;
            }
        }
        #endregion
    }
}
