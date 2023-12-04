using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data;
using NavOS.Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NavOS.Basecode.Services.ServiceModels;
using System.IO;
using NavOS.Basecode.Data.Repositories;
using NavOS.Basecode.Data.Models;

namespace NavOS.Basecode.Services.Services
{
    public class BookRequestService : IBookRequestService
    {
        private readonly IBookRequestRepository _bookRequestRepository;

        public BookRequestService(IBookRequestRepository bookRequestRepository, NavosDBContext dbContext)
        {
            _bookRequestRepository = bookRequestRepository;

        }
        public List<BookRequestViewModel> GetBooksRequest()
        {

            var data = _bookRequestRepository.GetBooksRequest().Select(s => new BookRequestViewModel
            {
                BookReqId = s.BookReqId,
                BookReqTitle = s.BookReqTitle,
                BookReqAuthor = s.BookReqAuthor,
                BookReqChapter = s.BookReqChapter,
                BookReqGenre = s.BookReqGenre,
                BookReqSource = s.BookReqSource,
                BookReqUserEmail = s.BookReqUserEmail,

            })
            .ToList();

            return data;
        }
        public BookRequestViewModel GetBookRequest(string BookReqId)
        {

            var book = _bookRequestRepository.GetBooksRequest().FirstOrDefault(s => s.BookReqId == BookReqId);
            var bookRequestViewModel = new BookRequestViewModel
            {
                BookReqId = book.BookReqId,
                BookReqTitle = book.BookReqTitle,
                BookReqAuthor = book.BookReqAuthor,
                BookReqChapter = book.BookReqChapter,
                BookReqGenre = book.BookReqGenre,
                BookReqSource = book.BookReqSource,
                BookReqUserEmail = book.BookReqUserEmail,

            };
            return bookRequestViewModel;
        }

        public void SendRequest(BookRequestViewModel book)
        {
            var model = new BookRequest
            {
                BookReqId = Guid.NewGuid().ToString(),
                //BookReqId = book.BookReqId,
                BookReqTitle = book.BookReqTitle,
                BookReqAuthor = book.BookReqAuthor,
                BookReqChapter = book.BookReqChapter,
                BookReqGenre = book.BookReqGenre,
                BookReqSource = book.BookReqSource,
                BookReqUserEmail = book.BookReqUserEmail,
            };
            //BookRequest.BookReqGenre = BookReqGenre;
            _bookRequestRepository.SendRequest(model);
        }

    }
}
