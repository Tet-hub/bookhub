using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        //get specific book details based on the bookService models
        public List<BookViewModel> GetBooks()
        {
            var data = _bookRepository.GetBooks().Select(s => new BookViewModel
            {
                BookId = s.BookId,
                BookTitle = s.BookTitle,
                Summary = s.Summary,
                Author = s.Author,
                Status = s.Status,
                Genre = s.Genre,
                Volume = s.Volume,
                DateReleased = s.DateReleased,
                AddedTime = s.AddedTime
            })
            .ToList();

            return data;
        }
        public BookViewModel GetBook(string BookId)
        {
            var book = _bookRepository.GetBooks().FirstOrDefault(s => s.BookId == BookId);

            if (book != null)
            {
                var bookViewModel = new BookViewModel
                {
                    BookId = book.BookId,
                    BookTitle = book.BookTitle,
                    Summary = book.Summary,
                    Author = book.Author,
                    Status = book.Status,
                    Genre = book.Genre,
                    Volume = book.Volume,
                    DateReleased = book.DateReleased,
                    AddedTime = book.AddedTime
                };
                return bookViewModel;
            }
            else
            {
                return null;
            }
        }

    }
}
