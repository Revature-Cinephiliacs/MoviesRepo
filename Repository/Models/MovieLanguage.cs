using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class MovieLanguage
    {
        public Guid MovieLanguageId { get; set; }
        public Guid MovieId { get; set; }
        public Guid LanguageId { get; set; }

        public virtual Language Language { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
