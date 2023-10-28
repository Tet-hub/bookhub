using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Interfaces
{
    public interface IGenreService
    {
        public List<GenreViewModel> GetGenres();
        public void AddGenre(GenreViewModel genre, string user);
        public bool Validate(string title);
        public GenreViewModel GetGenre(string GenreId);
    }
}
