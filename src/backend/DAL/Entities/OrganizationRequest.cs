namespace DAL.Entities
{
    public class OrganizationRequest
    {
        public int OrganizationRequestId { get; set; }
        public User User { get; set; } = null!;
        public Organization Organization { get; set; } = null!;
    }
}
