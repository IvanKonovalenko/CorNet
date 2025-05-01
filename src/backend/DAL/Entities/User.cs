namespace DAL.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public ICollection<UserOrganization> Organizations { get; set; } = new List<UserOrganization>();
        public ICollection<OrganizationRequest> OrganizationRequests { get; set; } = new List<OrganizationRequest>();
        public ICollection<Post> Likes { get; set; } = new List<Post>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }

}

