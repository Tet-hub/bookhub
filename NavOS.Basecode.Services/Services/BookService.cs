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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NavOS.Basecode.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreService _genreService;
        private readonly IReviewService _reviewService;
        public BookService(IBookRepository bookRepository,
            IGenreService genreService,
            IReviewService reviewService, NavosDBContext dbContext)
        {
            _bookRepository = bookRepository;
            _genreService = genreService;
            _reviewService = reviewService;
        }
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
                    result.Books = result.Books.OrderByDescending(book => book.TotalRating).ToList();
                    break;
            }
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
                Chapter = s.Chapter,
                DateReleased = s.DateReleased,
                AddedTime = s.AddedTime,
                ImageUrl = Path.Combine(url, s.BookId + ".png"),
                ReviewCount = s.Reviews.Count,
                TotalRating = s.Reviews.Any() ? Math.Round((double)s.Reviews.Sum(r => r.Rate) / s.Reviews.Count, 1) : 0,
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
            var url = "https://127.0.0.1:8080/";

            var booksData = _bookRepository.GetBooks()
                .Select(s => new BookViewModel
                {
                    BookId = s.BookId,
                    BookTitle = s.BookTitle,
                    Summary = s.Summary,
                    Author = s.Author,
                    Status = s.Status,
                    Genre = s.Genre,
                    Chapter = s.Chapter,
                    DateReleased = s.DateReleased,
                    AddedTime = s.AddedTime,
                    ImageUrl = Path.Combine(url, s.BookId + ".png"),
                    ReviewCount = s.Reviews.Count,
                    TotalRating = s.Reviews.Any() ? Math.Round((double)s.Reviews.Sum(r => r.Rate) / s.Reviews.Count, 1) : 0,
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
                .OrderByDescending(book => book.TotalRating)
                .Take(5)
                .ToList();

            bookWithReviews.LatestBooks = latestBooks;
            bookWithReviews.TopRatedBooks = topRatedBooks;

            return bookWithReviews;
        }

        /// <summary>
        /// Gets the book.
        /// </summary>
        /// <param name="BookId">The book identifier.</param>
        /// <returns></returns>
        public BookViewModel GetBook(string BookId)
        {
            var url = "https://127.0.0.1:8080/";
            var book = _bookRepository.GetBooks().FirstOrDefault(s => s.BookId == BookId);

            var bookViewModel = new BookViewModel
            {
                BookId = book.BookId,
                BookTitle = book.BookTitle,
                Summary = book.Summary,
                Author = book.Author,
                Status = book.Status,
                Genre = book.Genre,
                Chapter = book.Chapter,
                DateReleased = book.DateReleased,
                AddedTime = book.AddedTime,
                ImageUrl = Path.Combine(url, book.BookId + ".png"),

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
            var coverImagesPath = PathManager.DirectoryPath.CoverImagesDirectory;

            var model = new Book();
            model.BookId = Guid.NewGuid().ToString();
            model.BookTitle = book.BookTitle;
            model.Summary = book.Summary;
            model.Author = book.Author;
            model.Status = book.Status;
            model.Genre = book.Genre;
            model.Chapter = book.Chapter;
            model.DateReleased = book.DateReleased;
            model.AddedBy = user;
            model.UpdatedBy = user;
            model.AddedTime = DateTime.Now;
            model.UpdatedTime = DateTime.Now;

            if (book.ImageFile != null)
            {
                var coverImageFileName = Path.Combine(coverImagesPath, model.BookId) + ".png";
                using (var fileStream = new FileStream(coverImageFileName, FileMode.Create))
                {
                    book.ImageFile.CopyTo(fileStream);
                }
            }
            _bookRepository.AddBook(model);
        }

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

        public bool UpdateBook(BookViewModel bookViewModel, string user)
        {
            var coverImagesPath = PathManager.DirectoryPath.CoverImagesDirectory;
            Book book = _bookRepository.GetBooks().Where(x => x.BookId == bookViewModel.BookId).FirstOrDefault();
            if (book != null) 
            {
                book.BookTitle = bookViewModel.BookTitle;
                book.Summary = bookViewModel.Summary;
                book.Author = bookViewModel.Author;
                book.Status = bookViewModel.Status;
                book.Genre = bookViewModel.Genre;
                book.Chapter = bookViewModel.Chapter;
                book.DateReleased = bookViewModel.DateReleased;
                book.UpdatedBy = user;
                book.UpdatedTime = DateTime.Now;

                if (bookViewModel.ImageFile != null)
                {
                    var coverImageFileName = Path.Combine(coverImagesPath, book.BookId) + ".png";
                    using (var fileStream = new FileStream(coverImageFileName, FileMode.Create))
                    {
                        bookViewModel.ImageFile.CopyTo(fileStream);
                    }
                    _bookRepository.UpdateBook(book);
                    return true;
                }
                _bookRepository.UpdateBook(book);
                return true;
            }
            return false;
        }
        public bool Validate(string BookTitle)
        {
            var isExist = _bookRepository.GetBooks().Where(x => x.BookTitle == BookTitle).Any();
            return isExist;
        }

        //to be removed
        public FilteredBooksViewModel FilterAndSortBooks(string searchQuery = null, string filter = null, string sort = null)
        {
            var result = new FilteredBooksViewModel();
            result.Books = GetBooks();

            var currentDate = DateTime.Now;

            result.Books = result.Books
                .Where(book => book.AddedTime <= currentDate)
                .OrderByDescending(book => book.AddedTime)
                .ToList();

            ApplyFilter(new BookFilterOptions
            {
                SearchQuery = searchQuery,
                Filter = filter
            }, result);

            result.Genres = _genreService.GetGenres();
            return result;
        }
        /// <summary>
        /// Filters the and sort all book list.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public FilteredBooksViewModel FilterAndSortBookList(string searchQuery, string filter, string sort)
        {
            var result = new FilteredBooksViewModel();

            result.Books = GetBooks();

            // Genre filter
            if (!string.IsNullOrEmpty(filter) && string.Equals(filter.ToLower(), "genre", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(sort))
                {
                    // Filter by genre
                    result.Books = result.Books
                        .Where(book => book.Genre.Contains(sort, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Apply title search within the selected genre
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    result.Books = result.Books
                        .Where(book => book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }
            // All filters
            else if (string.IsNullOrEmpty(filter) || string.Equals(filter, "all", StringComparison.OrdinalIgnoreCase) || string.Equals(filter, "ratings", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    result.Books = result.Books
                        .Where(book =>
                            (book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }
            }
            //Filters based on the selected filter
            else if (!string.IsNullOrEmpty(searchQuery))
            {
                switch (filter.ToLower())
                {
                    case "title":
                        result.Books = result.Books
                            .Where(book => book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "author":
                        result.Books = result.Books
                            .Where(book => book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                }
            }

            //Sorting
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "title":
                        result.Books = result.Books.OrderBy(book => book.BookTitle, StringComparer.OrdinalIgnoreCase).ToList();
                        break;
                    case "author":
                        result.Books = result.Books.OrderBy(book => book.Author, StringComparer.OrdinalIgnoreCase).ToList();
                        break;
                    case "ratings":
                        result.Books = result.Books.OrderByDescending(book => book.TotalRating).ToList();
                        break;
                }
            }

            result.Genres = _genreService.GetGenres();

            return result;
        }

        /// <summary>
        /// Filters the and sort top book list.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public FilteredBooksViewModel FilterAndSortTopBookList(string searchQuery, string filter, string sort)
        {
            var result = new FilteredBooksViewModel();

            result.Books = GetBooks();
            // filters
            if (string.IsNullOrEmpty(filter) || string.Equals(filter, "ratings", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    result.Books = result.Books
                        .Where(book =>
                            (book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }
            }
            //search query
            else if (!string.IsNullOrEmpty(searchQuery))
            {
                switch (filter.ToLower())
                {
                    case "title":
                        result.Books = result.Books
                            .Where(book => book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "author":
                        result.Books = result.Books
                            .Where(book => book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "genre":
                        result.Books = result.Books
                            .Where(book => book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                }
            }
            //sorting
            if (string.Equals(sort, "title", StringComparison.OrdinalIgnoreCase))
            {
                result.Books = result.Books.OrderBy(book => book.BookTitle, StringComparer.OrdinalIgnoreCase).ToList();
            }
            else if (string.Equals(sort, "author", StringComparison.OrdinalIgnoreCase))
            {
                result.Books = result.Books.OrderBy(book => book.Author, StringComparer.OrdinalIgnoreCase).ToList();
            }
            else if (string.Equals(sort, "ratings", StringComparison.OrdinalIgnoreCase))
            {
                result.Books = result.Books.OrderByDescending(book => book.TotalRating).ToList();
            }
            else
            {
                result.Books = result.Books.OrderByDescending(book => book.TotalRating).ToList();
            }

            result.Genres = _genreService.GetGenres();

            return result;
        }

        /// <summary>
        /// Filters the and sort book list two weeks.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public FilteredBooksViewModel FilterAndSortBookListTwoWeeks(string searchQuery, string filter, string sort)
        {
            var result = new FilteredBooksViewModel();

            var data = GetBooks();

            var currentDate = DateTime.Now;
            var twoWeeksAgo = currentDate.AddDays(-14);

            data = data
                .Where(book => book.AddedTime >= twoWeeksAgo && book.AddedTime <= currentDate)
                .ToList();

            //filters
            if (string.IsNullOrEmpty(filter) || string.Equals(filter, "all", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    data = data
                        .Where(book =>
                            (book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }
            }
            //searchQuery
            else if (!string.IsNullOrEmpty(searchQuery))
            {
                switch (filter.ToLower())
                {
                    case "title":
                        data = data
                            .Where(book => book.BookTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "author":
                        data = data
                            .Where(book => book.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                    case "genre":
                        data = data
                            .Where(book => book.Genre.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        break;
                }
            }
            //sorting
            if (string.Equals(sort, "title", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.BookTitle, StringComparer.OrdinalIgnoreCase).ToList();
            }
            else if (string.Equals(sort, "author", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.Author, StringComparer.OrdinalIgnoreCase).ToList();
            }
            else if (string.Equals(sort, "newest", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderByDescending(book => book.AddedTime).ToList();
            }

            result.Books = data;
            result.Genres = _genreService.GetGenres();

            return result;
        }
        /// <summary>
        /// Gets the books for genre.
        /// </summary>
        /// <param name="genreName">Name of the genre.</param>
        /// <returns></returns>
        public List<BookViewModel> GetBooksForGenre(string genreName)
        {
            var url = "https://127.0.0.1:8080/";
            var data = _bookRepository.GetBooks().Where(s => s.Genre == genreName)
                .Select(s => new BookViewModel
                {
                    BookId = s.BookId,
                    BookTitle = s.BookTitle,
                    Summary = s.Summary,
                    Author = s.Author,
                    Status = s.Status,
                    Genre = s.Genre,
                    Chapter = s.Chapter,
                    DateReleased = s.DateReleased,
                    AddedTime = s.AddedTime,
                    ImageUrl = Path.Combine(url, s.BookId + ".png"),
                })
                .ToList();

            return data;
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
                var url = "https://127.0.0.1:8080/";
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
                        Chapter = book.Chapter,
                        DateReleased = book.DateReleased,
                        AddedTime = book.AddedTime,
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
        /// <summary>
        /// Filters the and sort all book list.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public FilteredBooksViewModel FilterAndSortAllBookList(string searchQuery = null,string filter = null,string sort = null)
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
            var reviewScore = 0.6;
            var ratingScore = 0.4;
            result.Books.ForEach(book => {
                double compositeScore = (reviewScore * book.ReviewCount) + (ratingScore * book.TotalRating);
                book.TotalScore = compositeScore;
                book.TotalRatingAndReviewsCount = book.TotalRating + book.ReviewCount + compositeScore;
            });
            result.Books = result.Books.OrderByDescending(book => book.TotalRatingAndReviewsCount).ToList();

            ApplyFilter(options, result);
            ApplySort(options, result);

            result.Genres = _genreService.GetGenres();
            return result;
        }
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


    }
}
