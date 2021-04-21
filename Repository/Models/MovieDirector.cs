using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class MovieDirector
    {
        public Guid MovieDirectorId { get; set; }
        public string ImdbId { get; set; }
        public Guid DirectorId { get; set; }

        public virtual Director Director { get; set; }
        public virtual Movie Imdb { get; set; }
    }
}
