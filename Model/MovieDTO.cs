using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model
{
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

        public override string ToString()
        {
            string output = ImdbId + "\n" + Title + "\n" + RatingName + "\n"
                + ReleaseDate + "\n" + ReleaseCountry + "\n" + RuntimeMinutes + "\n"
                + IsReleased + "\n" + Plot + "\n" + PosterURL + "\n";

            output += "Actors [\n";
            foreach (var actor in MovieActors)
            {
                output += "\t" + actor + "\n";
            }
            output += "]\n";

            output += "Directors [\n";
            foreach (var director in MovieDirectors)
            {
                output += "\t" + director + "\n";
            }
            output += "]\n";

            output += "Genres [\n";
            foreach (var genre in MovieGenres)
            {
                output += "\t" + genre + "\n";
            }
            output += "]\n";

            output += "Languages [\n";
            foreach (var language in MovieLanguages)
            {
                output += "\t" + language + "\n";
            }
            output += "]\n";

            output += "Tags [\n";
            foreach (var tag in MovieTags)
            {
                output += "\t" + tag + "\n";
            }
            output += "]\n";

            return output;
        }
        
    }
}
