﻿using System;
using System.Collections.Generic;

namespace NavOS.Basecode.Data.Models
{
    public partial class Genre
    {
        public string GenreId { get; set; }
        public string GenreName { get; set; }
        public string GenreDescription { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
