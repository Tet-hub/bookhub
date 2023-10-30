using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Data.Repositories;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public List<GenreViewModel> GetGenres()
        {
            var data = _genreRepository.GetGenre().Select(s => new GenreViewModel
            {
                GenreId = s.GenreId,
                GenreName = s.GenreName,
                GenreDescription = s.GenreDescription,
                UpdatedBy = s.UpdatedBy,
                AddedBy = s.AddedBy,
                
            })
            .ToList();

            return data;
        }
        public bool Validate(string title)
        {
            
            var isExist = _genreRepository.GetGenre().Where(x => x.GenreName == title).Any();
          

            //    var isExist = _genreRepository.GetGenre()
            //.Any(x => x.GenreName == title && x.GenreDescription == desc);

            return isExist;
        }
        //public GenreViewModel GetGenre(string GenreId)
        //{
        //    var genre = _genreRepository.GetGenre().FirstOrDefault(s => s.GenreId == GenreId);

        //    if (genre != null)
        //    {
        //        var genreViewModel = new GenreViewModel
        //        {
        //            GenreId = genre.GenreId,
        //            GenreName = genre.GenreName,
        //            GenreDescription = genre.GenreDescription,
        //            AddedBy = genre.AddedBy,
        //            UpdatedBy = genre.UpdatedBy,
        //            AddedTime = genre.AddedTime,
        //            UpdatedTime = genre.UpdatedTime,

        //        };
        //        return genreViewModel;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        public Genre GetGenre(string Genreid)
        {
            var genre = _genreRepository.GetGenre(Genreid);

            return genre;
        }


        public void AddGenre(GenreViewModel genre, string user)
        {
            
            var model = new Genre(); 
            model.GenreId = Guid.NewGuid().ToString(); 
            model.GenreName = genre.GenreName;
            model.GenreDescription = genre.GenreDescription;
            model.AddedBy = user;
            model.UpdatedBy = user;
            model.AddedTime = DateTime.Now;
            model.UpdatedTime = DateTime.Now;

            _genreRepository.AddGenre(model);

        }
        public bool UpdateGenre(GenreViewModel genreViewModel, string user)
        {
            Genre genre = (Genre)_genreRepository.GetGenre(genreViewModel.GenreId);
            if (genre != null)
            {
                genre.GenreName = genreViewModel.GenreName;
                genre.GenreDescription = genreViewModel.GenreDescription;
                genre.UpdatedBy = user;
                genre.UpdatedTime = System.DateTime.Now;

                _genreRepository.UpdateGenre(genre);
                return true;
            }

            return false;
        }

        public bool DeleteGenre(GenreViewModel genreViewModel)
        {
            Genre genre = _genreRepository.GetGenre(genreViewModel.GenreId);
            if (genre != null)
            {
                _genreRepository.DeleteGenre(genre);
                return true;
            }

            return false;
        }
    }
}
