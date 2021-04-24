using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class MovieActor
    {
        public string ImdbId { get; set; }
        public Guid ActorId { get; set; }
    }
}
