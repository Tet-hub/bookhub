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

namespace NavOS.Basecode.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository, NavosDBContext dbContext)
        {
            _bookRepository = bookRepository;
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
                TotalRating = s.Reviews.Any() ? Math.Round((double)s.Reviews.Sum(r => r.Rate) / s.Reviews.Count, 2) : 0,
        })
            .ToList();

            return data;
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

            var coverImageFileName = Path.Combine(coverImagesPath, model.BookId) + ".png";
            using (var fileStream = new FileStream(coverImageFileName, FileMode.Create))
            {
                book.ImageFile.CopyTo(fileStream);
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

        public List<BookViewModel> FilterAndSortBooks(string searchQuery, string filter, string sort)
        {
            var data = GetBooks();

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

            if (string.Equals(sort, "title", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.BookTitle, StringComparer.OrdinalIgnoreCase).ToList();
            }
            else if (string.Equals(sort, "author", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderBy(book => book.Author, StringComparer.OrdinalIgnoreCase).ToList();
            }
            else if (string.Equals(sort, "ratings", StringComparison.OrdinalIgnoreCase))
            {
                data = data.OrderByDescending(book => book.TotalRating).ToList();
            }
            return data;
        }

        public List<BookViewModel> FilterAndSortBooksTwoWeeks(string searchQuery, string filter, string sort, DateTime startDate, DateTime endDate)
        {
            var data = GetBooks();
            data = data
                .Where(book => book.AddedTime >= startDate && book.AddedTime <= endDate)
                .ToList();

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

            return data;
        }

        
    }
}
