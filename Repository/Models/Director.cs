﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Contains all information about a director.
    /// </summary>
    public partial class Director
    {
        public Director()
        {
            MovieDirectors = new HashSet<MovieDirector>();
        }

        public Guid DirectorId { get; set; }
        public string DirectorName { get; set; }

        public virtual ICollection<MovieDirector> MovieDirectors { get; set; }
    }
}
