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
            //to confirm if it exist
            //The LinQ is used below, retrieving data of All books in GetBooks(),
            //then it is filtered in Where if x is same "title" parameter
            var isExist = _genreRepository.GetGenre().Where(x => x.GenreName == title).Any();
            return isExist;
        }
        public void AddGenre(GenreViewModel genre, string user) // string user declared to retrieved the user in the controller
        {
            //Create a logic to create a model that will communicate the backend or in repository
            var model = new Genre(); // Why book is defined due to it is required to be placed in Db
                                     //Next to mapped the values base in the viewmodel
            model.GenreId = Guid.NewGuid().ToString(); // to generate random strings
            model.GenreName = genre.GenreName;
            model.GenreDescription = genre.GenreDescription;


            //Refered in controller
            model.AddedBy = user;
            model.UpdatedBy = user;
            model.AddedTime = DateTime.Now;
            model.UpdatedTime = DateTime.Now;

            //after setup model data, it is need to connect and pass into the repository
            _genreRepository.AddGenre(model);


            //Since there is still few conflicts due to the BookId is not set into int and not automatically incremented, in order to use the BookId for other references,
            //and also to make BookId generated in the Services
            //To implement the GUID to produce random strings
            // to create Guid declare the BookID pointed to GUID

        }

        public GenreViewModel GetGenre(string GenreId)
        {
            var genre = _genreRepository.GetGenre().FirstOrDefault(s => s.GenreId == GenreId);

            if (genre != null)
            {
                var genreViewModel = new GenreViewModel
                {
                    GenreId = genre.GenreId,
                    GenreName = genre.GenreName,
                    GenreDescription = genre.GenreDescription,
                    AddedBy = genre.AddedBy,
                    UpdatedBy = genre.UpdatedBy,
                    AddedTime = genre.AddedTime,
                    UpdatedTime = genre.UpdatedTime,
                    
                };
                return genreViewModel;
            }
            else
            {
                return null;
            }
        }

    }
}
