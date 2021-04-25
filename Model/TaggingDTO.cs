using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
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
        public bool? IsUpvote { get; set; }

        public TaggingDTO() {}   
    }
}
