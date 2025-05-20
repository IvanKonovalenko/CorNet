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

            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();

            if (user is null) throw new AuthorizationException();

            Organization organization = new Organization();
            organization.Code = organizationModel.Code;
            organization.Name = organizationModel.Name;

            UserOrganization userOrganization = new UserOrganization();
            userOrganization.Organization = organization;
            userOrganization.User = user;
            userOrganization.Role = Role.Owner;

            await _context.Organizations.AddAsync(organization);
            await _context.UserOrganizations.AddAsync(userOrganization);
            await _context.SaveChangesAsync();
        }
        public async Task SendRequest(string code, string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            if (await _context.UserOrganizations.
                AnyAsync(uo => uo.Organization.Code == code
                && uo.User.Email == email)) throw new UserAlreadyInOrganizationException();

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

            OrganizationRequest? organizationRequest = await _context.OrganizationRequests.Where(or => or.OrganizationRequestId == OrganizationRequestId).Include(or => or.User).FirstOrDefaultAsync();
            if (organizationRequest is null) throw new OrganizationRequestNotExistsException();

            User? userRequest = await _context.Users.Where(user => user.Email == organizationRequest.User.Email).FirstOrDefaultAsync();

            UserOrganization userOrganization = new UserOrganization() { User = userRequest!, Organization = organization, Role = Role.Member };

            _context.OrganizationRequests.Remove(organizationRequest);
            await _context.UserOrganizations.AddAsync(userOrganization);
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

            if (!await _context.UserOrganizations.
               AnyAsync(uo => uo.Organization == organization
               && uo.User == user)) throw new UserNotExistInOrganizationException();

            return await _context.UserOrganizations.Where(uo => uo.Organization.Code == code).Select(uo => new UserModel() {
                Email = uo.User.Email,
                Name = uo.User.Name,
                Surname = uo.User.Surname,
                Role = uo.Role.ToString()
            }).ToListAsync();
        }
        public async Task DeleteUser(string code, string email, string emailDeleteUser)
        {
            if(email==emailDeleteUser) throw new DeleteYourSelfException();

            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            if (!await _context.UserOrganizations.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user && (uo.Role == Role.Owner))) throw new RoleAccessException();

            UserOrganization? userOrganization = await _context.UserOrganizations.Where(uo => uo.User.Email == emailDeleteUser && uo.Organization.Code == code).FirstOrDefaultAsync();
            if (userOrganization is null) throw new UserNotExistInOrganizationException();

            

            _context.UserOrganizations.Remove(userOrganization);
            await _context.SaveChangesAsync();

        }
        public async Task ChangeRoleUser(string code, string email, string emailRoleUser, Role role)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            if (!await _context.UserOrganizations.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user && (uo.Role == Role.Owner))) throw new RoleAccessException();

            UserOrganization? userOrganization = await _context.UserOrganizations.Where(uo => uo.User.Email == emailRoleUser && uo.Organization.Code == code).FirstOrDefaultAsync();
            if (userOrganization is null) throw new UserNotExistInOrganizationException();
            if (userOrganization.Role == Role.Owner) throw new RoleOwnerException();

            userOrganization.Role = role;
            await _context.SaveChangesAsync();


        }
        private async Task ValidateRole(User user, Organization organization)
        {
            if (!await _context.UserOrganizations.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user && (uo.Role == Role.Owner || uo.Role == Role.Admin))) throw new RoleAccessException();
        }
        private async Task ValidateOrganization(string code)
        {
            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization?.OrganizationId != null)
                throw new DuplicateOrganizationException();
        }

        
    }
}
