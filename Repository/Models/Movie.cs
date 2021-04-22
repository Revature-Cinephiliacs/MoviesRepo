using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Movie
    {
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public int RatingId { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ReleaseCountry { get; set; }
        public int? RuntimeMinutes { get; set; }
        public bool? IsReleased { get; set; }
        public string Plot { get; set; }

        public virtual Rating Rating { get; set; }
    }
}
