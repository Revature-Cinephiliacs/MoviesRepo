using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Genre
    {
        public Genre()
        {
            MovieGenres = new HashSet<MovieGenre>();
        }

        public Guid GenreId { get; set; }
        public string GenreName { get; set; }

        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
    }
}
