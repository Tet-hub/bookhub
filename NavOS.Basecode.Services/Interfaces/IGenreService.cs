using NavOS.Basecode.Data.Models;
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
        #region GenreName Validation
        bool Validate(string title);
        bool ValidateForEdit(string title, string GenreId);
        #endregion
        List<GenreViewModel> GetGenres();
        void AddGenre(GenreViewModel genre, string user);   
        Genre GetGenre(string Genreid);
        bool UpdateGenre(GenreViewModel genreViewModel, string user);
        bool DeleteGenre(string GenreId);
        List<GenreViewModel> GetGenresWithBook(string searchTerm);

    }
}
