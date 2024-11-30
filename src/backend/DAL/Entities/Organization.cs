public class Organization
{
    public int OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public ICollection<User> Users { get; set; }
}