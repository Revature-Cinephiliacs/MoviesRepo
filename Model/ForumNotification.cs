using System.Collections.Generic;
using System.Linq;
namespace Model
{
    /// <summary>
    /// Facillitates passing of notifications.
    /// </summary>
    /// <value></value>
    public class ForumNotification
    {
        public string Imdbid { get; set; }
        public string Usernameid { get; set; }
        public string DiscussionId { get; set; }
        public List<string> Followers { get; set; }
    }
}