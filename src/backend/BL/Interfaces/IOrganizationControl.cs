using BL.Models.Organization;
using DAL.Entities;

namespace BL.Interfaces
{
    public interface IOrganizationControl
    {
        Task Create(OrganizationModel organizationModel, string email);
        Task<OrganizationRequest> SendRequest(string code, string email);
        Task<List<OrganizationRequestModel>> ShowRequests(string code, string email);
        Task AcceptRequest(int OrganizationRequestId, string code, string email);
        Task RefuseRequest(int OrganizationRequestId, string code, string email);
        Task<List<UserModel>> ShowUsers(string code, string email);
        Task DeleteUser(string code, string email, string emailDeleteUser);
        Task ChangeRoleUser(string code, string email, string emailRoleUser, Role role);
    }
}
