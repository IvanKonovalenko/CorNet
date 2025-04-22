namespace DAL.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public User User { get; set; } = null!;
        public Post Post { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime dateTime { get; set; } 
    }
}
