using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public sealed class TaggingDTO
    {
        [Required]
        [StringLength(20)]
        public string Movieid { get; set; }
        [Required]
        [StringLength(50)]
        public string Userid { get; set; }
        [Required]
        [StringLength(50)]
        public string Tagname { get; set; }
        public bool? Isupvote { get; set; }

        public TaggingDTO() {}   
    }
}
