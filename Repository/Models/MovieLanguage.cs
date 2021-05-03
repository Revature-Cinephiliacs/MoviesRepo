using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Associates a language with a movie.
    /// </summary>
    public partial class MovieLanguage
    {
        public string ImdbId { get; set; }
        public Guid LanguageId { get; set; }

        public virtual Movie Imdb { get; set; }
        public virtual Language Language { get; set; }
    }
}
