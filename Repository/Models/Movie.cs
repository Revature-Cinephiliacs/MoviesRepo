using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Movie
    {
        public Movie()
        {
            MovieActors = new HashSet<MovieActor>();
            MovieDirectors = new HashSet<MovieDirector>();
            MovieGenres = new HashSet<MovieGenre>();
            MovieLanguages = new HashSet<MovieLanguage>();
            MovieTagUsers = new HashSet<MovieTagUser>();
            MovieTags = new HashSet<MovieTag>();
        }

        public string ImdbId { get; set; }
        public string Title { get; set; }
        public int? RatingId { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ReleaseCountry { get; set; }
        public short? RuntimeMinutes { get; set; }
        public bool? IsReleased { get; set; }
        public string Plot { get; set; }
        public string PosterUrl { get; set; }

        public virtual Rating Rating { get; set; }
        public virtual ICollection<MovieActor> MovieActors { get; set; }
        public virtual ICollection<MovieDirector> MovieDirectors { get; set; }
        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
        public virtual ICollection<MovieLanguage> MovieLanguages { get; set; }
        public virtual ICollection<MovieTagUser> MovieTagUsers { get; set; }
        public virtual ICollection<MovieTag> MovieTags { get; set; }
    }
}
