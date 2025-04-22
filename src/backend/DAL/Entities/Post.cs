namespace DAL.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime dateTime { get; set; }
        public Organization Organization { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<User> Likes { get; set; } = new List<User>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}

