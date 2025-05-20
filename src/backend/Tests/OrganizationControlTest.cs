using BL.Exceptions;
using BL.Models.Organization;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class OrganizationControlTest : BaseTest
    {
        [Fact]
        public async Task Create()
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            var organizationModel = new OrganizationModel
            {
                Name = "Organization",
                Code = "Org2"
            };
            await _organizationControl.Create(organizationModel, user.Email);
            Organization? organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Code == organizationModel.Code);

            Assert.NotNull(organization);
            
            await Assert.ThrowsAsync<DuplicateOrganizationException>(() => _organizationControl.Create(organizationModel, user.Email));

            var organizationModel1 = new OrganizationModel
            {
                Name = "Organization",
                Code = "AnotherOrg"
            };

            await Assert.ThrowsAsync<AuthorizationException>(() => _organizationControl.Create(organizationModel1, "anotheremail@example.com"));            
        }
        [Fact]
        public async Task SendRequest()
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test7@example.com");
            await _organizationControl.SendRequest("Org1", user.Email);
            OrganizationRequest? organizationRequest = await _context.OrganizationRequests
                                                        .Include(or => or.User)
                                                        .Include(or => or.Organization)
                                                        .FirstOrDefaultAsync(or => or.User.Email == "test7@example.com"
                                                        && or.Organization.Code == "Org1");

            Assert.NotNull(organizationRequest);

            await Assert.ThrowsAsync<RequestExistsException>(() => _organizationControl.SendRequest("Org1", user.Email));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _organizationControl.SendRequest("AnotherOrg", user.Email));

            await Assert.ThrowsAsync<AuthorizationException>(() => _organizationControl.SendRequest("Org1", "anotheremail@example.com"));

            await Assert.ThrowsAsync<UserAlreadyInOrganizationException>(() => _organizationControl.SendRequest("Org1", "test1@example.com"));
        }
        [Fact]
        public async Task ShowRequests()
        {
            List<OrganizationRequestModel> organizationRequests = await _organizationControl.ShowRequests("Org1", "test@example.com");

            Assert.True(organizationRequests?.Any(or => or.Email == "test4@example.com" && or.Code == "Org1" && or.Name == "Test" && or.Surname == "User"));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _organizationControl.ShowRequests("AnotherOrg", "test@example.com"));

            await Assert.ThrowsAsync<AuthorizationException>(() => _organizationControl.ShowRequests("Org1", "anotheremail@example.com"));

            await Assert.ThrowsAsync<RoleAccessException>(() => _organizationControl.ShowRequests("Org1", "test1@example.com"));
        }
        [Fact]
        public async Task AcceptRequest()
        {
            OrganizationRequest? organizationRequest = await _context.OrganizationRequests
                                                        .Include(or => or.User)
                                                        .Include(or => or.Organization)
                                                        .FirstOrDefaultAsync(or => or.User.Email == "test4@example.com"
                                                        && or.Organization.Code == "Org1");

            await Assert.ThrowsAsync<RoleAccessException>(() => _organizationControl.AcceptRequest(organizationRequest.OrganizationRequestId, "Org1", "test1@example.com"));

            await _organizationControl.AcceptRequest(organizationRequest.OrganizationRequestId, "Org1", "test@example.com");
            OrganizationRequest? organizationRequest1 = await _context.OrganizationRequests
                                                        .Include(or => or.User)
                                                        .Include(or => or.Organization)
                                                        .FirstOrDefaultAsync(or => or.User.Email == "test4@example.com"
                                                        && or.Organization.Code == "Org1");

            Assert.Null(organizationRequest1);

            UserOrganization? userOrganization = await _context.UserOrganizations
                                        .Include(ur => ur.User)
                                        .Include(ur => ur.Organization)
                                        .FirstOrDefaultAsync(uo => uo.User.Email == "test4@example.com" 
                                        && uo.Organization.Code == "Org1");
            
            Assert.NotNull(userOrganization);

            await Assert.ThrowsAsync<OrganizationRequestNotExistsException>(() =>_organizationControl.AcceptRequest(organizationRequest.OrganizationRequestId, "Org1", "test@example.com"));

            await Assert.ThrowsAsync<AuthorizationException>(() => _organizationControl.AcceptRequest(organizationRequest.OrganizationRequestId, "Org1", "anotheremail@example.com"));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _organizationControl.AcceptRequest(organizationRequest.OrganizationRequestId, "AnotherOrg", "test@example.com"));
        }
        [Fact]
        public async Task RefuseRequest()
        {
            OrganizationRequest? organizationRequest = await _context.OrganizationRequests
                                                        .Include(or => or.User)
                                                        .Include(or => or.Organization)
                                                        .FirstOrDefaultAsync(or => or.User.Email == "test5@example.com"
                                                        && or.Organization.Code == "Org1");

            await Assert.ThrowsAsync<RoleAccessException>(() => _organizationControl.RefuseRequest(organizationRequest.OrganizationRequestId, "Org1", "test1@example.com"));

            await _organizationControl.RefuseRequest(organizationRequest.OrganizationRequestId, "Org1", "test@example.com");
            OrganizationRequest? organizationRequest1 = await _context.OrganizationRequests
                                                        .Include(or => or.User)
                                                        .Include(or => or.Organization)
                                                        .FirstOrDefaultAsync(or => or.User.Email == "test5@example.com"
                                                        && or.Organization.Code == "Org1");

            Assert.Null(organizationRequest1);

            UserOrganization? userOrganization = await _context.UserOrganizations
                                        .Include(ur => ur.User)
                                        .Include(ur => ur.Organization)
                                        .FirstOrDefaultAsync(uo => uo.User.Email == "test5@example.com"
                                        && uo.Organization.Code == "Org1");

            Assert.Null(userOrganization);

            await Assert.ThrowsAsync<OrganizationRequestNotExistsException>(() => _organizationControl.RefuseRequest(organizationRequest.OrganizationRequestId, "Org1", "test@example.com"));

            await Assert.ThrowsAsync<AuthorizationException>(() => _organizationControl.RefuseRequest(organizationRequest.OrganizationRequestId, "Org1", "anotheremail@example.com"));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _organizationControl.RefuseRequest(organizationRequest.OrganizationRequestId, "AnotherOrg", "test@example.com"));
        }
        [Fact]
        public async Task ShowUsers()
        {
            List<UserModel> users = await _organizationControl.ShowUsers("Org1", "test1@example.com");

            Assert.True(users?.Any(u => u.Email == "test@example.com" && u.Name == "Test" && u.Surname == "User" && u.Role=="Owner"));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _organizationControl.ShowUsers("AnotherOrg", "test1@example.com"));

            await Assert.ThrowsAsync<AuthorizationException>(() => _organizationControl.ShowUsers("Org1", "anotheremail@example.com"));

            await Assert.ThrowsAsync<UserNotExistInOrganizationException>(() => _organizationControl.ShowUsers("Org1", "test8@example.com"));
        }
        [Fact]
        public async Task DeleteUser()
        {
            await _organizationControl.DeleteUser("Org1", "test@example.com", "test2@example.com");

            UserOrganization? userOrganization = await _context.UserOrganizations
                                        .Include(ur => ur.User)
                                        .Include(ur => ur.Organization)
                                        .FirstOrDefaultAsync(uo => uo.User.Email == "test2@example.com"
                                        && uo.Organization.Code == "Org1");

            Assert.Null(userOrganization);

            await Assert.ThrowsAsync<AuthorizationException>(() => _organizationControl.DeleteUser("Org1", "anotheremail@example.com", "test2@example.com"));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _organizationControl.DeleteUser("AnotherOrg", "test@example.com", "test2@example.com"));

            await Assert.ThrowsAsync<RoleAccessException>(() => _organizationControl.DeleteUser("Org1", "test1@example.com", "test2@example.com"));

            await Assert.ThrowsAsync<UserNotExistInOrganizationException>(() => _organizationControl.DeleteUser("Org1", "test@example.com", "test4@example.com"));

            await Assert.ThrowsAsync<DeleteYourSelfException>(() => _organizationControl.DeleteUser("Org1", "test@example.com", "test@example.com"));
        }
        [Fact]
        public async Task ChangeRoleUser()
        {
            await _organizationControl.ChangeRoleUser("Org1", "test@example.com", "test3@example.com", Role.Admin);

            UserOrganization? userOrganization = await _context.UserOrganizations
                                       .Include(ur => ur.User)
                                       .Include(ur => ur.Organization)
                                       .FirstOrDefaultAsync(uo => uo.User.Email == "test3@example.com"
                                       && uo.Organization.Code == "Org1");

            Assert.True(userOrganization.Role==Role.Admin);

            await Assert.ThrowsAsync<AuthorizationException>(() => _organizationControl.ChangeRoleUser("Org1", "anotheremail@example.com", "test3@example.com", Role.Admin));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _organizationControl.ChangeRoleUser("AnotherOrg", "test@example.com", "test3@example.com", Role.Admin));

            await Assert.ThrowsAsync<RoleAccessException>(() => _organizationControl.ChangeRoleUser("Org1", "test1@example.com", "test3@example.com", Role.Admin));

            await Assert.ThrowsAsync<UserNotExistInOrganizationException>(() => _organizationControl.ChangeRoleUser("Org1", "test@example.com", "test4@example.com", Role.Admin));

            await Assert.ThrowsAsync<RoleOwnerException>(() => _organizationControl.ChangeRoleUser("Org1", "test@example.com", "test@example.com", Role.Admin));
        }

    }
}
