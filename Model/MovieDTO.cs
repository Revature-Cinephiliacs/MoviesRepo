using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public sealed class MovieDTO
    {
        [Required]
        [StringLength(20)]
        public string ImdbId { get; set; }
        
        public string Title { get; set; }
        
        public string RatingName { get; set; }

        [RegularExpression( @"[12]\d\d\d-[01]\d-[0123]\d")]
        public string ReleaseDate { get; set; }
        public string ReleaseCountry { get; set; }
        public short? RuntimeMinutes { get; set; }
        public bool? IsReleased { get; set; }
        public string Plot { get; set; }
        public string PosterURL { get; set; }
        public List<string> MovieActors { get; set; }
        public List<string> MovieDirectors { get; set; }
        public List<string> MovieGenres { get; set; }
        public List<string> MovieLanguages { get; set; }
        public List<string> MovieTags { get; set; }

        public MovieDTO() {}
        
    }
}
