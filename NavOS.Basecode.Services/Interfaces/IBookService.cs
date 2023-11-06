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
        public BookViewModel GetBook(string BookId);
        void AddBook(BookViewModel book, string user);
        bool Validate(string BookTitle);
        bool DeleteBook(string bookId);
        bool UpdateBook(BookViewModel bookViewModel, string user);
        List<BookViewModel> FilterAndSortBooks(string searchQuery, string filter, string sort);
        List<BookViewModel> FilterAndSortBooksTwoWeeks(string searchQuery, string filter, string sort, DateTime startDate, DateTime endDate);
    }
}
