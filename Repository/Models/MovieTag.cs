using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Keeps track of the sum of votes for/against associating a
    /// specific tag with a specific movie.
    /// </summary>
    public partial class MovieTag
    {
        public string ImdbId { get; set; }
        public string TagName { get; set; }
        public int VoteSum { get; set; }

        public virtual Movie Imdb { get; set; }
        public virtual Tag TagNameNavigation { get; set; }
    }
}
