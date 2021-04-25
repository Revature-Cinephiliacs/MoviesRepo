using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public sealed class MovieDTO : IEquatable<MovieDTO>
        {

            [Required]
            [StringLength(20)]
            public string ImdbId { get; set; }
            [Required]
            [StringLength(35)]
            public string Title { get; set; }
            
            public DateTime? ReleaseDate { get; set; }
            public string ReleaseCountry { get; set; }
            public short? RuntimeMinutes { get; set; }
            public bool? IsReleased { get; set; }

            [Required]
            [StringLength(350)]
            public string Plot { get; set; }

            public MovieDTO() {}

            public MovieDTO(string ImdbId, string Title, DateTime? ReleaseDate, string ReleaseCountry, short? RuntimeMinutes, bool? IsReleased, string Plot)
            {
                this.ImdbId = ImdbId;
                this.Title = Title;
                this.ReleaseDate = ReleaseDate;
                this.ReleaseCountry = ReleaseCountry;
                this.RuntimeMinutes = RuntimeMinutes;
                this.IsReleased = IsReleased;
                this.Plot = Plot;
            }


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
                return Equals(obj as MovieDTO);
            }

            public override int GetHashCode()
            {
                return ImdbId.GetHashCode();
            }
        
    }
}
