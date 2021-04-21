using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class MovieGenre
    {
        public Guid MovieGenreId { get; set; }
        public Guid MovieId { get; set; }
        public Guid GenreId { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
