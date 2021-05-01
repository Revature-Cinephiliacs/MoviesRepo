using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Rating
    {
        public Rating()
        {
            Movies = new HashSet<Movie>();
        }

        public int RatingId { get; set; }
        public string RatingName { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }
    }
}
