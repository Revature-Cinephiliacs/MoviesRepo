using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class MovieActor
    {
        public Guid MovieActorId { get; set; }
        public Guid MovieId { get; set; }
        public Guid ActorId { get; set; }

        public virtual Actor Actor { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
