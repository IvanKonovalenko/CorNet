using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Post;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class PostControl : BaseService, IPostControl
    {
        public PostControl(AppDbContext context) : base(context)
        {

        }
        public async Task CreatePost(string text, string code, string email)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await ValidateRole(user, organization);

            Post post = new Post();
            post.Text = text;
            post.User = user;
            post.Organization = organization;
            post.dateTime = DateTime.UtcNow;

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

        }
        public async Task DeletePost(string code, string email, int postId)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await ValidateRole(user, organization);
            Post? post = await CheckPostNotExsits(postId);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
        public async Task<List<PostModel>> ShowPosts(string code, string email)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await CheckUserNotExistInOrganization(organization, user);

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
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await CheckUserNotExistInOrganization(organization, user);
            Post? post = await CheckPostNotExsits(postId);

            return await _context.Comments.Where(c => c.PostId == postId).Select(c => new CommentModel()
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                Email = c.User.Email,
                Code = code,
                dateTime = c.dateTime,
                Text = c.Text

            }).ToListAsync();


        }
        public async Task CreateComment(string code, string email, int postId, string text)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await CheckUserNotExistInOrganization(organization, user);
            Post? post = await CheckPostNotExsits(postId);

            Comment comment = new Comment();
            comment.Post = post;
            comment.Text = text;
            comment.User = user;
            comment.dateTime = DateTime.UtcNow;

            await _context.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(string code, string email, int postId, int commentId)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            await ValidateRole(user, organization);
            Post? post = await CheckPostNotExsits(postId);
            Comment? comment = await CheckCommentExsits(commentId);

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

        }
        public async Task Like(string code, string email, int postId)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            Post? post = await CheckPostNotExsits(postId);
            await CheckUserNotExistInOrganization(organization, user);

            if (!post.Likes.Any(u => u.UserId == user.UserId))
            {
                post.Likes.Add(user);
                await _context.SaveChangesAsync();
            }


        }

        public async Task DisLike(string code, string email, int postId)
        {
            User? user = await CheckAuthorization(email);
            Organization? organization = await CheckOrganization(code);
            Post? post = await CheckPostNotExsits(postId);
            await CheckUserNotExistInOrganization(organization, user);

            if (post.Likes.Any(u => u.UserId == user.UserId))
            {
                post.Likes.Remove(user);
                await _context.SaveChangesAsync();
            }

        }
    }
}
