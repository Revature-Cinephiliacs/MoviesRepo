using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Tag
    {
        public Tag()
        {
            MovieTagUsers = new HashSet<MovieTagUser>();
            MovieTags = new HashSet<MovieTag>();
        }

        public string TagName { get; set; }
        public bool IsBanned { get; set; }

        public virtual ICollection<MovieTagUser> MovieTagUsers { get; set; }
        public virtual ICollection<MovieTag> MovieTags { get; set; }
    }
}
