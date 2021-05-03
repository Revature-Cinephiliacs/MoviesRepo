using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Relates a movie to a user who is following that movie.
    /// </summary>
    public partial class FollowingMovie
    {
        public string UserId { get; set; }
        public string ImdbId { get; set; }

        public virtual Movie Imdb { get; set; }
    }
}
