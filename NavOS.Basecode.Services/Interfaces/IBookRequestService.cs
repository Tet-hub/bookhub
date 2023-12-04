using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Interfaces
{
    public interface IBookRequestService
    {
        public List<BookRequestViewModel> GetBooksRequest();
        public BookRequestViewModel GetBookRequest(string BookReqId);
        public void SendRequest(BookRequestViewModel book);
    }
}
