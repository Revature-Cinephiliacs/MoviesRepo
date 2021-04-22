using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class MovieLanguage
    {
        public string ImdbId { get; set; }
        public Guid LanguageId { get; set; }
    }
}
