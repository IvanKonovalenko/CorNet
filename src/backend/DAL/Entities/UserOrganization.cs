namespace DAL.Entities
{
    public class UserOrganization
    {
        public int UserOrganizationId { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public User User { get; set; } = null!;
        public Organization Organization { get; set; } = null!;
        public Role Role { get; set; }
    }
}
