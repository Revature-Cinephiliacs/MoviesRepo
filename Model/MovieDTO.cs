using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// Contains the movie information that is sent from and returned
    /// to the frontend.
    /// </summary>
    public sealed class MovieDTO
    {
        [StringLength(20)]
        public string ImdbId { get; set; }
        
        [StringLength(255)]
        public string Title { get; set; }
        
        [StringLength(255)]
        public string RatingName { get; set; }

        [RegularExpression( @"[12]\d\d\d-[01]\d-[0123]\d")]
        public string ReleaseDate { get; set; }

        [StringLength(255)]
        public string ReleaseCountry { get; set; }
        public short? RuntimeMinutes { get; set; }
        public bool? IsReleased { get; set; }
        public string Plot { get; set; }

        [StringLength(2048)]
        public string PosterURL { get; set; }
        public List<string> MovieActors { get; set; }
        public List<string> MovieDirectors { get; set; }
        public List<string> MovieGenres { get; set; }
        public List<string> MovieLanguages { get; set; }
        public List<string> MovieTags { get; set; }

        public MovieDTO() {}
        
    }
}
