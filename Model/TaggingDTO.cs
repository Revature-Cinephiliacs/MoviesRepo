using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// Contains the information required for a user to tag a movie.
    /// This is passed into the TagMovie endpoint.
    /// </summary>
    public sealed class TaggingDTO
    {
        [Required]
        [StringLength(20)]
        public string MovieId { get; set; }
        [Required]
        [StringLength(50)]
        public string UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string TagName { get; set; }
        [Required]
        public bool IsUpvote { get; set; }

        public TaggingDTO() {}
    }
}
