using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Associates a director with a movie.
    /// </summary>
    public partial class MovieDirector
    {
        public string ImdbId { get; set; }
        public Guid DirectorId { get; set; }

        public virtual Director Director { get; set; }
        public virtual Movie Imdb { get; set; }
    }
}
