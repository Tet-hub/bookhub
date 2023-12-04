using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.ServiceModels
{
    public class BookRequestViewModel
    {
        public string BookReqId { get; set; }
        public string BookReqTitle { get; set; }
        public string BookReqAuthor { get; set; }
        public string BookReqChapter { get; set; }
        public string BookReqGenre { get; set; }
        public string BookReqSource { get; set; }
        public string BookReqUserEmail { get; set; }

        public DateTime BookReqTime { get; set; }

    }
}
