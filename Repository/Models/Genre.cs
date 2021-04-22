using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Genre
    {
        public Guid GenreId { get; set; }
        public string GenreName { get; set; }
    }
}
