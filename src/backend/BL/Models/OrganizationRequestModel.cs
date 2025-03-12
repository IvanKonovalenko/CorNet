namespace BL.Models
{
    public class OrganizationRequestModel
    {
        public int OrganizationRequestId { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
