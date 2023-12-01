using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Interfaces
{
    public interface IBookService
    {
        List<BookViewModel> GetBooks();
        BookViewModel GetBook(string BookId);
        void AddBook(BookViewModel book, string user);
        bool DeleteBook(string bookId);
        bool UpdateBook(BookViewModel bookViewModel, string user);
        BookWithReviewViewModel GetBooksWithReviews();
        BookWithReviewViewModel GetBookWithReviews(string bookId);

        #region Filter Books
        FilteredBooksViewModel FilterAndSortBooks(string searchQuery = null, string filter = null, string sort = null);
        FilteredBooksViewModel FilterAndSortAllBookList(string searchQuery = null, string filter = null, string sort = null);
        FilteredBooksViewModel FilterAndSortTopBooks(string searchQuery = null, string filter = null, string sort = null);
        FilteredBooksViewModel FilterAndSortNewBooks(string searchQuery = null, string filter = null, string sort = null);
        #endregion

        #region Validate BookTitles
        bool Validate(string BookTitle);
        bool ValidateForEdit(string BookTitle, string bookId);
        #endregion
    }
}
