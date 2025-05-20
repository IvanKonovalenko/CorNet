using BL.Exceptions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class BaseService
    {
        protected readonly AppDbContext _context;
        public BaseService(AppDbContext context)
        {
            _context = context;
        }
        protected async Task<User> CheckAuthorization(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null) throw new AuthorizationException();
            return user;
        }
        protected async Task<Organization> CheckOrganization(string code)
        {
            Organization? organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Code == code);
            if (organization is null) throw new OrganizationNotExistsException();
            return organization;
        }
        protected async Task CheckUserAlreadyInOrganization(string email, string code)
        {
            if (await _context.UserOrganizations.
                AnyAsync(uo => uo.Organization.Code == code
                && uo.User.Email == email)) throw new UserAlreadyInOrganizationException();
        }
        protected async Task CheckRequestExists(string email, string code)
        {
            if (await _context.OrganizationRequests.
                AnyAsync(or => or.Organization.Code == code
                && or.User.Email == email)) throw new RequestExistsException();
        }
        protected async Task<UserOrganization> CheckUserNotExistInOrganization(Organization organization, User user)
        {
            UserOrganization? userOrganization = await _context.UserOrganizations.FirstOrDefaultAsync(uo => uo.Organization == organization && uo.User == user);
            if (userOrganization is null) throw new UserNotExistInOrganizationException();
            return userOrganization;
        }
        protected async Task<Comment> CheckCommentExsits(int commentId)
        {
            Comment? comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
            if (comment is null) throw new CommentNotExistException();
            return comment;
        }
        protected void CheckDeleteYourSelf(string email, string emailDeleteUser)
        {
            if (email == emailDeleteUser) throw new DeleteYourSelfException();
        }
        protected void CheckRoleNotOwner(UserOrganization userOrganization)
        {
            if (userOrganization.Role == Role.Owner) throw new RoleOwnerException();
        }
        protected async Task<OrganizationRequest> CheckOrganizationRequestNotExists(int organizationRequestId)
        {
            OrganizationRequest? organizationRequest = await _context.OrganizationRequests.Include(or => or.User).FirstOrDefaultAsync(or => or.OrganizationRequestId == organizationRequestId);
            if (organizationRequest is null) throw new OrganizationRequestNotExistsException();
            return organizationRequest;
        }
        protected async Task ValidateRole(User user, Organization organization)
        {
            if (!await _context.UserOrganizations.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user && (uo.Role == Role.Owner || uo.Role == Role.Admin))) throw new RoleAccessException();
        }
        protected async Task ValidateOwner(User user, Organization organization)
        {
            if (!await _context.UserOrganizations.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user && uo.Role == Role.Owner)) throw new RoleAccessException(); 
        }
        protected async Task ValidateOrganization(string code)
        {
            var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Code == code);
            if (organization?.OrganizationId != null)
                throw new DuplicateOrganizationException();
        }
        protected async Task ValidateEmail(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
            if (user != null)
                throw new DuplicateEmailException();
        }
        protected async Task<User> CheckUserExist(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null) throw new UserNotExistException();
            return user;
        }
        protected async Task<User> CheckReceiver(string receiver)
        {
            User? userReceiver = await _context.Users.Where(u => u.Email == receiver).FirstOrDefaultAsync();
            if (userReceiver is null) throw new ReceiverException();
            return userReceiver;
        }
        protected async Task<Post> CheckPostNotExsits(int postId)
        {
            Post? post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.PostId == postId);
            if (post is null) throw new PostNotExistException();
            return post;
        }

    }
}
