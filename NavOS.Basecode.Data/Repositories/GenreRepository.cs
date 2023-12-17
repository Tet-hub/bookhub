using Basecode.Data.Repositories;
using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Data.Repositories
{
    public class GenreRepository : BaseRepository, IGenreRepository
    {
        public GenreRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        //retrieve all genre
        public IQueryable<Genre> GetGenre()
        {
            return this.GetDbSet<Genre>();
        }
        ///retrieve single genre
        public Genre GetGenre(string Genreid)
        {
            var genre = this.GetDbSet<Genre>().FirstOrDefault(x => x.GenreId == Genreid);
            return genre;
        }
        public void AddGenre(Genre genre)
        {
            this.GetDbSet<Genre>().Add(genre);
            UnitOfWork.SaveChanges();
        }
        public void UpdateGenre(Genre genre)
        {
            this.GetDbSet<Genre>().Update(genre);
            UnitOfWork.SaveChanges();
        }

        public void DeleteGenre(Genre genre)
        {
            this.GetDbSet<Genre>().Remove(genre);
            UnitOfWork.SaveChanges();
        }


    }
}
