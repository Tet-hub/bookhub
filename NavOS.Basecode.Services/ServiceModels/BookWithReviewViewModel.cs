using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.ServiceModels
{
    public class BookWithReviewViewModel
    {
        public List<BookViewModel> Books { get; set; }
        public List<ReviewViewModel> Reviews { get; set; }
        public List<BookViewModel> LatestBooks { get; set; }
        public List<BookViewModel> TopRatedBooks { get; set; }
        public BookViewModel BookDetails { get; set; }
        public ReviewViewModel Review { get; set; }
    }
}
