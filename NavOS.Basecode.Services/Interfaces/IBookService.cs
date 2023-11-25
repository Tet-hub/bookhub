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
        bool Validate(string BookTitle);
        bool DeleteBook(string bookId);
        bool UpdateBook(BookViewModel bookViewModel, string user);
        List<BookViewModel> FilterAndSortBooks(string filter, string searchQuery, string sort);
        List<BookViewModel> GetBooksForGenre(string genreName);
        FilteredBooksViewModel FilterAndSortBookList(string searchQuery, string filter, string sort);
        FilteredBooksViewModel FilterAndSortBookListTwoWeeks(string searchQuery, string filter, string sort);
        BookWithReviewViewModel GetBooksWithReviews();
        BookWithReviewViewModel GetBookWithReviews(string bookId);
        FilteredBooksViewModel FilterAndSortTopBookList(string searchQuery, string filter, string sort);


        FilteredBooksViewModel FilterAndSortAllBookList(string searchQuery = null, string filter = null, string sort = null);
        FilteredBooksViewModel FilterAndSortTopBooks(string searchQuery = null, string filter = null, string sort = null);
        FilteredBooksViewModel FilterAndSortNewBooks(string searchQuery = null, string filter = null, string sort = null);
    }
}
