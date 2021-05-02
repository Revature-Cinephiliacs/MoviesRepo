using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Model
{
    public class ReviewNotification
    {
        /// <summary>
        /// Facillitates passing of notifications.
        /// </summary>
        /// <value></value>
        public string Imdbid { get; set; }
        public string Usernameid { get; set; }
        public Guid Reviewid { get; set; }
        public List<string> Followers { get; set; }
        
    }
}