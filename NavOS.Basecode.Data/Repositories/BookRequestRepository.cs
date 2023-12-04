using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Data.Repositories
{
    public class BookRequestRepository : BaseRepository, IBookRequestRepository
    {
        public BookRequestRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public IQueryable<BookRequest> GetBooksRequest()
        {
            return this.GetDbSet<BookRequest>();
        }


        public IQueryable<BookRequest> GetBookRequest(string BookReqId)
        {
            var bookReq = this.GetDbSet<BookRequest>().Where(x => x.BookReqId == BookReqId);
            return bookReq;
        }
        public void SendRequest(BookRequest book)
        {
            this.GetDbSet<BookRequest>().Add(book);
            UnitOfWork.SaveChanges();
        }

    }
}
