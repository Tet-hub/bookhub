using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.ServiceModels
{
    public class FilteredBooksViewModel
    {
        public List<BookViewModel> Books { get; set; }
        public List<GenreViewModel> Genres { get; set; }
    }

}
