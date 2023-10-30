using NavOS.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Data.Interfaces
{
    public interface IGenreRepository
    {
        public IQueryable<Genre> GetGenre();
        //public IQueryable<Genre> GetGenre(string GenreId);
        ///retrieve single genre
        public Genre GetGenre(string Genreid);

        public void AddGenre(Genre genre);
        public void UpdateGenre(Genre genre);
        public void DeleteGenre(Genre genre);
    }
}
