namespace DAL.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public string Text { get; set; } = null!;
        public Organization Organization { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<File> Files { get; set; } = new List<File>();
        public ICollection<User> Likes { get; set; } = new List<User>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}

