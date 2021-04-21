using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class MovieLanguage
    {
        public Guid MovieLanguageId { get; set; }
        public string ImdbId { get; set; }
        public Guid LanguageId { get; set; }

        public virtual Movie Imdb { get; set; }
        public virtual Language Language { get; set; }
    }
}
