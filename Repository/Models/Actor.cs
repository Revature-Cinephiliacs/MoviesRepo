using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Contains all information about an actor.
    /// </summary>
    public partial class Actor
    {
        public Actor()
        {
            MovieActors = new HashSet<MovieActor>();
        }

        public Guid ActorId { get; set; }
        public string ActorName { get; set; }

        public virtual ICollection<MovieActor> MovieActors { get; set; }
    }
}
