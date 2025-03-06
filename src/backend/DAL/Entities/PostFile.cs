namespace DAL.Entities
{
    public class PostFile
    {
        public int PostId { get; set; }
        public int FileId { get; set; }
        public Post Post { get; set; } = null!;
        public File File { get; set; } = null!;
    }
}
