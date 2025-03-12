using BL.Models;
using DAL.Entities;

namespace BL.Interfaces
{
    public interface IOrganizationControl
    {
        Task<Organization> CreateOrganization(OrganizationModel organizationModel, string email);
        Task<OrganizationRequest> SendRequestOrganization(string code, string email);
        Task<List<OrganizationRequestModel>> ShowRequestsOrganization(string code, string email);

    }
}
