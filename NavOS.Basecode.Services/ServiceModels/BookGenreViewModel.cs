using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.ServiceModels
{
    public class BookGenreViewModel
    {
        public BookViewModel Book { get; set; }
        public List<GenreViewModel> Genres { get; set; }
    }
}
