using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Organization;
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
        public async Task Create(OrganizationModel organizationModel, string email)
        {
            await ValidateOrganization(organizationModel.Code);

            Organization organization = new Organization();
            organization.Code = organizationModel.Code;
            organization.Name = organizationModel.Name;

            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();

            if (user is null) throw new AuthorizationException();

            UserOrganization userOrganization = new UserOrganization();
            userOrganization.Organization = organization;
            userOrganization.User = user;
            userOrganization.Role = Role.Owner;

            await _context.Organizations.AddAsync(organization);
            await _context.UserOrganization.AddAsync(userOrganization);
            await _context.SaveChangesAsync();
        }
        public async Task SendRequest(string code, string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            if (await _context.OrganizationRequests.
                AnyAsync(or => or.Organization.Code == code
                && or.User.Email == email)) throw new RequestExistsException();

            OrganizationRequest organizationRequest = new OrganizationRequest();
            organizationRequest.User = user;
            organizationRequest.Organization = organization;

            await _context.OrganizationRequests.AddAsync(organizationRequest);
            await _context.SaveChangesAsync();

        }
        public async Task<List<OrganizationRequestModel>> ShowRequests(string code, string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            await ValidateRole(user, organization);

            return await _context.OrganizationRequests.Where(uo => uo.Organization.Code == code).Select(uo=>new OrganizationRequestModel() { 
                OrganizationRequestId = uo.OrganizationRequestId,
                Email = uo.User.Email,
                Name = uo.User.Name,
                Surname = uo.User.Surname,
                Code = uo.Organization.Code
            }).ToListAsync();

        }
        public async Task AcceptRequest(int OrganizationRequestId, string code, string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            await ValidateRole(user, organization);

            OrganizationRequest? organizationRequest = await _context.OrganizationRequests.Where(or => or.OrganizationRequestId == OrganizationRequestId).FirstOrDefaultAsync();
            if (organizationRequest is null) throw new OrganizationRequestNotExistsException();

            UserOrganization userOrganization = new UserOrganization() { User = organizationRequest.User, Organization = organization, Role = Role.Member };

            _context.OrganizationRequests.Remove(organizationRequest);
            await _context.UserOrganization.AddAsync(userOrganization);
            await _context.SaveChangesAsync();
        }
        public async Task RefuseRequest(int OrganizationRequestId, string code, string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            await ValidateRole(user, organization);

            OrganizationRequest? organizationRequest = await _context.OrganizationRequests.Where(or => or.OrganizationRequestId == OrganizationRequestId).FirstOrDefaultAsync();
            if (organizationRequest is null) throw new OrganizationRequestNotExistsException();

            _context.OrganizationRequests.Remove(organizationRequest);
            await _context.SaveChangesAsync();
        }
        public async Task<List<UserModel>> ShowUsers(string code, string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();


            return await _context.UserOrganization.Where(uo => uo.Organization.Code == code).Select(uo => new UserModel() {
                Email = uo.User.Email,
                Name = uo.User.Name,
                Surname = uo.User.Surname
            }).ToListAsync();
        }
        public async Task DeleteUser(string code, string email, string emailDeleteUser)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            await ValidateRole(user, organization);

            UserOrganization? userOrganization = await _context.UserOrganization.Where(uo => uo.User.Email == emailDeleteUser && uo.Organization.Code == code).FirstOrDefaultAsync();
            if (userOrganization is null) throw new UserNotExistInOrganizationException();
            if (userOrganization.Role != Role.Member) throw new RoleAccessException();

            _context.UserOrganization.Remove(userOrganization);
            await _context.SaveChangesAsync();

        }
        public async Task ChangeRoleUser(string code, string email, string emailRoleUser, Role role)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            if (!await _context.UserOrganization.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user && (uo.Role == Role.Owner))) throw new RoleAccessException();

            UserOrganization? userOrganization = await _context.UserOrganization.Where(uo => uo.User.Email == emailRoleUser && uo.Organization.Code == code).FirstOrDefaultAsync();
            if (userOrganization is null) throw new UserNotExistInOrganizationException();
            if (userOrganization.Role == Role.Owner) throw new RoleAccessException();

            userOrganization.Role = role;
            await _context.SaveChangesAsync();


        }
        public async Task ValidateRole(User user, Organization organization)
        {
            if (!await _context.UserOrganization.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user && (uo.Role == Role.Owner || uo.Role == Role.Admin))) throw new RoleAccessException();
        }
        public async Task ValidateOrganization(string code)
        {
            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization?.OrganizationId != null)
                throw new DuplicateOrganizationException();
        }

        
    }
}
