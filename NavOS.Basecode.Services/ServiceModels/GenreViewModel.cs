using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.ServiceModels
{
    public class GenreViewModel
    {
        
        public string GenreId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string GenreName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string GenreDescription { get; set; }
        public string UpdatedBy { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

    }
}
