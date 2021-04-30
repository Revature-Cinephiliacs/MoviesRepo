using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Associates a genre with a movie.
    /// </summary>
    public partial class MovieGenre : IEquatable<MovieGenre>
    {
        public string ImdbId { get; set; }
        public Guid GenreId { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual Movie Imdb { get; set; }

        public bool Equals(MovieGenre other)
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

            return GenreId == other.GenreId;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as MovieGenre);
        }

        public static bool operator ==(MovieGenre lhs, MovieGenre rhs)
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

        public static bool operator !=(MovieGenre lhs, MovieGenre rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return GenreId.GetHashCode();
        }
        
    }
}
