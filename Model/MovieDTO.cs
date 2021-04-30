using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// Contains the movie information that is sent from and returned
    /// to the frontend.
    /// </summary>
    public sealed class MovieDTO : IEquatable<MovieDTO>
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

        public bool Equals(MovieDTO other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return ImdbId == other.ImdbId;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as MovieDTO);
        }

        public static bool operator ==(MovieDTO lhs, MovieDTO rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(MovieDTO lhs, MovieDTO rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return ImdbId.GetHashCode();
        }
        
    }
}
