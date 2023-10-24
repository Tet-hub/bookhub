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
        //get all books
        IQueryable<Book> GetBooks();
        //get single book
        IQueryable<Book> GetBook(string BookId);
    }
}
