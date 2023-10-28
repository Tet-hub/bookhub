using NavOS.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.ServiceModels
{
    public class ReviewViewModel
    {
        public string BookId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string UserEmail { get; set; }
        public string ReviewText { get; set; }
        public int Rate { get; set; }
        public DateTime DateReviewed { get; set; }
    }
}
