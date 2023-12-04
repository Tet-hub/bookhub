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
        /// <summary>
        /// Gets the genres.
        /// </summary>
        /// <returns></returns>
        public List<GenreViewModel> GetGenres()
        {
            var data = _genreRepository.GetGenre().Select(s => new GenreViewModel
            {
                GenreId = s.GenreId,
                GenreName = s.GenreName,
                GenreDescription = s.GenreDescription,
                UpdatedBy = s.UpdatedBy,
                AddedBy = s.AddedBy,
                UpdatedTime = s.UpdatedTime,
                AddedTime = s.AddedTime,
            })
            .OrderBy(g => g.GenreName)
            .ToList();

            return data;
        }
        /// <summary>
        /// Gets the genres with book.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <returns></returns>
        public List<GenreViewModel> GetGenresWithBook(string searchQuery)
        {
            var data = _genreRepository.GetGenre().Select(s => new GenreViewModel
            {
                GenreId = s.GenreId,
                GenreName = s.GenreName,
                GenreDescription = s.GenreDescription,
                UpdatedBy = s.UpdatedBy,
                AddedBy = s.AddedBy,
                UpdatedTime = s.UpdatedTime,
                AddedTime = s.AddedTime,

            })
            .OrderBy(g => g.GenreName)
            .ToList();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                data = ApplySearch(data, searchQuery);
            }

            return data;
        }
        /// <summary>
        /// Gets the genre.
        /// </summary>
        /// <param name="Genreid">The genreid.</param>
        /// <returns></returns>
        public Genre GetGenre(string Genreid)
        {
            var genre = _genreRepository.GetGenre(Genreid);

            return genre;
        }
        /// <summary>
        /// Adds the genre.
        /// </summary>
        /// <param name="genre">The genre.</param>
        /// <param name="user">The user.</param>
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
        /// <summary>
        /// Updates the genre.
        /// </summary>
        /// <param name="genreViewModel">The genre view model.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Deletes the genre.
        /// </summary>
        /// <param name="GenreId">The genre identifier.</param>
        /// <returns></returns>
        public bool DeleteGenre(string GenreId)
        {
            Genre genre = _genreRepository.GetGenre(GenreId);
            if (genre != null)
            {
                _genreRepository.DeleteGenre(genre);
                return true;
            }

            return false;
        }

        #region Validate GenreName
        /// <summary>
        /// Validates the genre title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public bool Validate(string title)
        {
            var isExist = _genreRepository.GetGenre().Where(x => x.GenreName == title).Any();
            return isExist;
        }
        /// <summary>
        /// Validates for genre title during editing.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="GenreId">The genre identifier.</param>
        /// <returns></returns>
        public bool ValidateForEdit(string title, string GenreId)
        {
            var isExist = _genreRepository.GetGenre()
                            .Any(x => x.GenreName == title && x.GenreId != GenreId.ToString());
            return isExist;
        }
        #endregion
        #region private methods        
        /// <summary>
        /// Applies the search.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="searchQuery">The search query.</param>
        /// <returns></returns>
        private List<GenreViewModel> ApplySearch(List<GenreViewModel> data, string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return data;
            }

            searchQuery = searchQuery.ToLowerInvariant();

            return data
                .Where(g => g.GenreName.ToLowerInvariant().Contains(searchQuery))
                .ToList();
        }
        #endregion
    }
}
