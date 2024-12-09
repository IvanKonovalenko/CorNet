public interface IAuth
{
    Task<User> CreateUser(AuthModel user);
    Task<User> Authenticate(string email, string password);
    Task ValidateEmail(string email);
    Task Register(AuthModel user);
    Task<Organization> ValidateOrganization(string code);
}