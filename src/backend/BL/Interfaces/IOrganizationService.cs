public interface IOrganizationService
{
    Task CreateOrganization(OrganizationModel model);
    Task<List<UserModel>> GetUsers(string code);
}