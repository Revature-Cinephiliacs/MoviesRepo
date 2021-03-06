using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Contains all information about a language.
    /// </summary>
    public partial class Language
    {
        public Language()
        {
            MovieLanguages = new HashSet<MovieLanguage>();
        }

        public Guid LanguageId { get; set; }
        public string LanguageName { get; set; }

        public virtual ICollection<MovieLanguage> MovieLanguages { get; set; }
    }
}
