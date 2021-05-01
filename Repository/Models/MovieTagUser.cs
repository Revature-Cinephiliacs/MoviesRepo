using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
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
