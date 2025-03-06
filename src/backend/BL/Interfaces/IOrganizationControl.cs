using BL.Models;
using DAL.Entities;

namespace BL.Interfaces
{
    public interface IOrganizationControl
    {
        Task<Organization> CreateOrganization(OrganizationModel organizationModel, string email);
        Task<OrganizationRequest> SendRequestOrganization(OrganizationRequestModel organizationRequestModel, string email);
    }
}
