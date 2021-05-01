using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class FollowingMovie
    {
        public string UserId { get; set; }
        public string ImdbId { get; set; }

        public virtual Movie Imdb { get; set; }
    }
}
