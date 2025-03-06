using BL.Exceptions;
using BL.Interfaces;
using BL.Models;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL
{
    public class OrganizationControl : IOrganizationControl
    {
        private readonly AppDbContext _context;
        public OrganizationControl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Organization> CreateOrganization(OrganizationModel organizationModel, string email)
        {
            await ValidateOrganization(organizationModel.Code);

            Organization organization = new Organization();
            organization.Code = organizationModel.Code;
            organization.Name = organizationModel.Name;

            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();

            if (user is null) throw new AuthorizationExeception();

            UserOrganization userOrganization = new UserOrganization();
            userOrganization.Organization = organization;
            userOrganization.User = user;
            userOrganization.Role = Role.Owner;

            await _context.Organizations.AddAsync(organization);
            await _context.UserOrganization.AddAsync(userOrganization);
            await _context.SaveChangesAsync();
            return organization;
        }

        public async Task<OrganizationRequest> SendRequestOrganization(OrganizationRequestModel organizationRequestModel, string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationExeception();

            var organization = await _context.Organizations.Where(o => o.Code == organizationRequestModel.Code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExsistsException();

            if (await _context.OrganizationRequests.
                AnyAsync(or => or.Organization.Code == organizationRequestModel.Code
                && or.User.Email == email)) throw new RequestExsistsException();

            OrganizationRequest organizationRequest = new OrganizationRequest();
            organizationRequest.User = user;
            organizationRequest.Organization = organization;
            return organizationRequest;

        }

        public async Task ValidateOrganization(string code)
        {
            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization?.OrganizationId != null)
                throw new DuplicateOrganizationException();
        }
    }
}
