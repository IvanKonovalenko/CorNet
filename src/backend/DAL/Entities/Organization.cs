namespace DAL.Entities
{
    public class Organization
    {
        public int OrganizationId { get; set; }
        public string Name { get; set; } = null!;
        public File? Photo { get; set; }
        public string Code { get; set; } = null!;
        public ICollection<UserOrganization> Users { get; set; } = new List<UserOrganization>();
        public ICollection<OrganizationRequest> OrganizationRequests { get; set; } = new List<OrganizationRequest>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
