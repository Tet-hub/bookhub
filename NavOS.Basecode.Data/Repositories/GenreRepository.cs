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
        //retrieve single genre
        public IQueryable<Genre> GetGenre(string GenreId)
        {
            var genre = this.GetDbSet<Genre>().Where(x => x.GenreId == GenreId);
            return genre;
        }
        public void AddGenre(Genre genre)
        {
            this.GetDbSet<Genre>().Add(genre);
            UnitOfWork.SaveChanges(); //Do not forget to include this line
        }

    }
}
