using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class MovieDirector
    {
        public string ImdbId { get; set; }
        public Guid DirectorId { get; set; }
    }
}
