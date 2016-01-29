namespace TinyCms.Web.Models.Posts
{
    public class FacebookStats
    {
        public string url { get; set; }
        public int share_count { get; set; }
        public int like_count { get; set; }
        public int comment_count { get; set; }
        public int total_count { get; set; }
        public int click_count { get; set; }
        public string comments_fbid { get; set; }
        public int commentsbox_count { get; set; }
    }
}