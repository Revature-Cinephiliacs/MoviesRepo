using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Contains a word, whether that word qualifies as a tag,
    /// and that word's root word.
    /// </summary>
    public partial class Word
    {
        public string Word1 { get; set; }
        public bool IsTag { get; set; }
        public string BaseWord { get; set; }
    }
}
