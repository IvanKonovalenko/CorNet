namespace BL.Models.Post
{
    public class PostModel
    {
        public int PostId { get; set; }
        public string Text { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public DateTime dateTime { get; set; }
        public int Likes { get; set; }
        public bool IsLiked { get; set; }
    }
}
