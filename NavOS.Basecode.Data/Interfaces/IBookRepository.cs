using NavOS.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Data.Interfaces
{
    public interface IBookRepository
    {
        IQueryable<Book> GetBooks();
        IQueryable<Book> GetBook(string BookId);
        void DeleteBook(Book book);
        void AddBook(Book book);
        void UpdateBook(Book book);
    }
}
