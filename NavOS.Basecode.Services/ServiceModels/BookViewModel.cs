using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.ServiceModels
{
    public class BookViewModel
    {
        public string BookId { get; set; }
        [Required(ErrorMessage = "Book Title is required")]
        public string BookTitle { get; set; }
        [Required(ErrorMessage = "Summary is required")]
        public string Summary { get; set; }
        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }
        [Required(ErrorMessage = "Genre is required")]
        public string Genre { get; set; }
        [Required(ErrorMessage = "Please Choose a Genre")]
        public List<string> SelectedGenres { get; set; }
        [Required(ErrorMessage = "Chapter is required")]
        public string Chapter { get; set; }
        //[Required(ErrorMessage = "Please upload an image")]
        public IFormFile ImageFile { get; set; }
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Date Released is required")]
        public DateTime DateReleased { get; set; }

        public DateTime AddedTime { get; set; }

        public string Source { get; set; }

        public int ReviewCount { get; set; }
        public double TotalRating { get; set;}
        public double AverageRate { get; set; }
        public List<ReviewViewModel> Reviews { get; set; }
        public double TotalRatingAndReviewsCount { get; set; }
        public double TotalScore { get; set; }

        //populate the genre
        public List<GenreViewModel> Genres { get; set; }
    }
}
