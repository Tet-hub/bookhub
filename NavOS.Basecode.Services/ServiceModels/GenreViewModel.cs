using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.ServiceModels
{
    public class GenreViewModel
    {
        public string GenreId { get; set; }
        public string GenreName { get; set; }
        public string GenreDescription { get; set; }
        public string UpdatedBy { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

    }
}
