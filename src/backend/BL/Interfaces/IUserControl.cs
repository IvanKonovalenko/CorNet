using BL.Models.Organization;

namespace BL.Interfaces
{
    public interface IUserControl
    {
        Task<UserModel> ShowProfile(string email);
        Task<List<OrganizationModel>> ShowOrganizations(string email);
    }
}
