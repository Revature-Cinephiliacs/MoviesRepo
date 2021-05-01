namespace Model
{
    public class ForumNotification
    {
        /// <summary>
        /// Facillitates passing of notifications.
        /// </summary>
        /// <value></value>
        public string Imdbid { get; set; }
        public string Usernameid { get; set; }
        public string DiscussionId { get; set; }
        public List<string> Followers { get; set; }
    }
}