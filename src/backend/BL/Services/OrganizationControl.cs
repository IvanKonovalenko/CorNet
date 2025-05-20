using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Organization;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class OrganizationControl : BaseService, IOrganizationControl
    {
        public OrganizationControl(AppDbContext context) : base(context) 
        {
        }
        public async Task Create(OrganizationModel organizationModel, string email)
        {
            await ValidateOrganization(organizationModel.Code);
            User? user = await CheckAuthorization(email);

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
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await CheckUserAlreadyInOrganization(email, code);
            await CheckRequestExists(email, code);

            OrganizationRequest organizationRequest = new OrganizationRequest();
            organizationRequest.User = user;
            organizationRequest.Organization = organization;

            await _context.OrganizationRequests.AddAsync(organizationRequest);
            await _context.SaveChangesAsync();

        }
        public async Task<List<OrganizationRequestModel>> ShowRequests(string code, string email)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await ValidateRole(user, organization);

            return await _context.OrganizationRequests.Where(uo => uo.Organization.Code == code).Select(uo=>new OrganizationRequestModel() { 
                OrganizationRequestId = uo.OrganizationRequestId,
                Email = uo.User.Email,
                Name = uo.User.Name,
                Surname = uo.User.Surname,
                Code = uo.Organization.Code
            }).ToListAsync();

        }
        public async Task AcceptRequest(int organizationRequestId, string code, string email)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await ValidateRole(user, organization);
            OrganizationRequest? organizationRequest = await CheckOrganizationRequestNotExists(organizationRequestId);

            User? userRequest = await _context.Users.FirstOrDefaultAsync(user => user.Email == organizationRequest.User.Email);

            UserOrganization userOrganization = new UserOrganization() { User = userRequest!, Organization = organization, Role = Role.Member };

            _context.OrganizationRequests.Remove(organizationRequest);
            await _context.UserOrganizations.AddAsync(userOrganization);
            await _context.SaveChangesAsync();
        }
        public async Task RefuseRequest(int organizationRequestId, string code, string email)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await ValidateRole(user, organization);
            OrganizationRequest? organizationRequest = await CheckOrganizationRequestNotExists(organizationRequestId);

            _context.OrganizationRequests.Remove(organizationRequest);
            await _context.SaveChangesAsync();
        }
        public async Task<List<UserModel>> ShowUsers(string code, string email)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await CheckUserNotExistInOrganization(organization, user);

            return await _context.UserOrganizations.Where(uo => uo.Organization.Code == code).Select(uo => new UserModel() {
                Email = uo.User.Email,
                Name = uo.User.Name,
                Surname = uo.User.Surname,
                Role = uo.Role.ToString()
            }).ToListAsync();
        }
        public async Task DeleteUser(string code, string email, string emailDeleteUser)
        {
            CheckDeleteYourSelf(email, emailDeleteUser);
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await ValidateOwner(user, organization);
            User? userDelete = await CheckAuthorization(emailDeleteUser);
            UserOrganization? userOrganization = await CheckUserNotExistInOrganization(organization, userDelete);

            _context.UserOrganizations.Remove(userOrganization);
            await _context.SaveChangesAsync();

        }
        public async Task ChangeRoleUser(string code, string email, string emailRoleUser, Role role)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await ValidateOwner(user, organization);
            User? userChangeRole = await CheckAuthorization(emailRoleUser);
            UserOrganization? userOrganization = await CheckUserNotExistInOrganization(organization, userChangeRole);
            CheckRoleNotOwner(userOrganization);

            userOrganization.Role = role;
            await _context.SaveChangesAsync();
        }           
    }
}
