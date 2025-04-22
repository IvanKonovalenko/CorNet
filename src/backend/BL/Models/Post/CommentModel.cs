namespace BL.Models.Post
{
    public class CommentModel
    {
        public int CommentId { get; set; }
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int PostId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime dateTime { get; set; }
    }
}
