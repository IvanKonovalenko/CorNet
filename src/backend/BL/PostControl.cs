using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Organization;
using BL.Models.Post;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL
{
    public class PostControl : IPostControl
    {
        private readonly AppDbContext _context;
        public PostControl(AppDbContext context)
        {
            _context = context;
        }
        public async Task Create(CreatePostModel model, string code, string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            await ValidateRole(user, organization);

            Post post = new Post();
            post.Text = model.Text;
            post.dateTime = model.dateTime;
            post.User = user;
            post.Organization = organization;
            post.dateTime = DateTime.Now;

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

        }
        public async Task<List<PostModel>> ShowPosts(string code, string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            if (!await _context.UserOrganization.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user)) throw new UserNotExistInOrganizationException();

            return await _context.Posts.Where(p => p.Organization.Code == code).Select(p => new PostModel()
            {
                PostId = p.PostId,
                Text = p.Text,
                Email = p.User.Email,
                Code = p.Organization.Code,
                dateTime = p.dateTime,
                Likes = p.Likes.Count
            }).ToListAsync();
        }
        public Task<List<CommentModel>> ShowComments(string code, string email, int postId)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

 


        }

        public async Task ValidateRole(User user, Organization organization)
        {
            if (!await _context.UserOrganization.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user && (uo.Role == Role.Owner || uo.Role == Role.Admin))) throw new RoleAccessException();
        }
    }
}
