using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Associates a genre with a movie.
    /// </summary>
    public partial class MovieGenre
    {
        public string ImdbId { get; set; }
        public Guid GenreId { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual Movie Imdb { get; set; }
    }
}
