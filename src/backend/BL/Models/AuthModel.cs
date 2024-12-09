public class AuthModel
{
    public string Email { get; set; } = null!;
    public string Name { get; set; }=null!;
    public string Surname { get; set; }=null!;
    public string Password { get; set; } = null!;
    public string OrganizationCode { get; set; }=null!;
    public Role Role { get; set; }
}