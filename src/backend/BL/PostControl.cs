using BL.Exceptions;
using BL.Interfaces;
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
        public async Task CreatePost(CreatePostModel model, string code, string email)
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
        public async Task DeletePost(string code, string email, int postId)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            var post = await _context.Posts.Where(p => p.PostId == postId).FirstOrDefaultAsync();
            if (post is null) throw new PostNotExistException();

            await ValidateRole(user, organization);

            _context.Posts.Remove(post);
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
                Likes = p.Likes.Count,
                IsLiked = p.Likes.Any(l => l.UserId == user.UserId)
            }).ToListAsync();
        }
        public async Task<List<CommentModel>> ShowComments(string code, string email, int postId)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            if (!await _context.UserOrganization.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user)) throw new UserNotExistInOrganizationException();

            var post = await _context.Posts.Where(p => p.PostId == postId).FirstOrDefaultAsync();
            if (post is null) throw new PostNotExistException();

            return await _context.Comments.Where(c => c.PostId == postId).Select(c => new CommentModel()
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                Email = email,
                Code = code,
                dateTime = c.dateTime,
                Text = c.Text

            }).ToListAsync();


        }
        public async Task CreateComment(string code, string email, int postId, string text)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            if (!await _context.UserOrganization.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user)) throw new UserNotExistInOrganizationException();

            var post = await _context.Posts.Where(p => p.PostId == postId).FirstOrDefaultAsync();
            if (post is null) throw new PostNotExistException();

            Comment comment = new Comment();
            comment.Post = post;
            comment.Text = text;
            comment.User = user;
            comment.dateTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(string code, string email, int postId, int CommentId)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            var post = await _context.Posts.Where(p => p.PostId == postId).FirstOrDefaultAsync();
            if (post is null) throw new PostNotExistException();

            var comment = await _context.Comments.Where(c => c.CommentId == CommentId).FirstOrDefaultAsync();
            if (post is null) throw new CommentNotExistException();

            await ValidateRole(user, organization);

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

        }
        public async Task Like(string code, string email, int postId)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            var post = await _context.Posts.Include(p => p.Likes).Where(p => p.PostId == postId).FirstOrDefaultAsync();
            if (post is null) throw new PostNotExistException();

            if (!await _context.UserOrganization.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user)) throw new UserNotExistInOrganizationException();

            if (!post.Likes.Any(u => u.UserId == user.UserId))
            {
                post.Likes.Add(user);
                await _context.SaveChangesAsync();
            }


        }

        public async Task DisLike(string code, string email, int postId)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new AuthorizationException();

            var organization = await _context.Organizations.Where(o => o.Code == code).FirstOrDefaultAsync();
            if (organization is null) throw new OrganizationNotExistsException();

            var post = await _context.Posts.Include(p => p.Likes).Where(p => p.PostId == postId).FirstOrDefaultAsync();
            if (post is null) throw new PostNotExistException();

            if (!await _context.UserOrganization.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user)) throw new UserNotExistInOrganizationException();

            if (post.Likes.Any(u => u.UserId == user.UserId))
            {
                post.Likes.Remove(user);
                await _context.SaveChangesAsync();
            }

        }


        public async Task ValidateRole(User user, Organization organization)
        {
            if (!await _context.UserOrganization.
                AnyAsync(uo => uo.Organization == organization
                && uo.User == user && (uo.Role == Role.Owner || uo.Role == Role.Admin))) throw new RoleAccessException();
        }
 
    }
}
