using System;
using System.Collections.Generic;

namespace NavOS.Basecode.Data.Models
{
    public partial class BookRequest
    {
        public string BookReqId { get; set; }
        public string BookReqTitle { get; set; }
        public string BookReqAuthor { get; set; }
        public string BookReqChapter { get; set; }
        public string BookReqGenre { get; set; }
        public string BookReqSource { get; set; }
        public string BookReqUserEmail { get; set; }
    }
}
