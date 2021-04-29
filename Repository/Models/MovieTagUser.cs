using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Is a vote, by a user, for or against associating a given
    /// tag with a specific movie.
    /// </summary>
    public partial class MovieTagUser
    {
        public string ImdbId { get; set; }
        public string TagName { get; set; }
        public string UserId { get; set; }
        public bool? IsUpvote { get; set; }

        public virtual Movie Imdb { get; set; }
        public virtual Tag TagNameNavigation { get; set; }
    }
}
