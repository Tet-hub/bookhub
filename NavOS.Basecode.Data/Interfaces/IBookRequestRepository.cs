using NavOS.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Data.Interfaces
{
    public interface IBookRequestRepository
    {
        public IQueryable<BookRequest> GetBooksRequest();
        public IQueryable<BookRequest> GetBookRequest(string BookReqId);
        public void SendRequest(BookRequest book);

    }
}
