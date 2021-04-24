using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class MovieGenre
    {
        public string ImdbId { get; set; }
        public Guid GenreId { get; set; }
    }
}
