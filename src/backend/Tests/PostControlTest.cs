using BL.Exceptions;
using BL.Models.Post;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class PostControlTest : BaseTest
    {
        [Fact]
        public async Task CreatePost()
        {
            await _postControl.CreatePost("createPost", "Org1", "test@example.com");
            Post? post = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com" 
                            && p.Organization.Code == "Org1" && p.Text == "createPost");

            Assert.NotNull(post);

            await Assert.ThrowsAsync<AuthorizationException>(() => _postControl.CreatePost("createPost", "Org1", "anotheremail@example.com"));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _postControl.CreatePost("createPost", "AnotherOrg", "test@example.com"));

            await Assert.ThrowsAsync<RoleAccessException>(() => _postControl.CreatePost("createPost", "Org1", "test1@example.com"));
        }
        [Fact]
        public async Task DeletePost()
        {

            Post? post = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com"
                            && p.Organization.Code == "Org1" && p.Text == "DeletePost");            
            await _postControl.DeletePost("Org1", "test@example.com", post.PostId);
            Post? deletedPost = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com"
                            && p.Organization.Code == "Org1" && p.Text == "DeletePost");

            Assert.Null(deletedPost);

            await Assert.ThrowsAsync<AuthorizationException>(() => _postControl.DeletePost("Org1", "anotheremail@example.com", post.PostId));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _postControl.DeletePost("AnotherOrg", "test@example.com", post.PostId));

            await Assert.ThrowsAsync<RoleAccessException>(() => _postControl.DeletePost("Org1", "test1@example.com", post.PostId));

            await Assert.ThrowsAsync<PostNotExistException>(() => _postControl.DeletePost("Org1", "test@example.com", post.PostId));
        }
        [Fact]
        public async Task ShowPosts()
        {
            List<PostModel> posts = await _postControl.ShowPosts("Org1", "test@example.com");

            Assert.True(posts?.Any(p => p.Email == "test@example.com" && p.Code == "Org1" && p.Text == "ShowPost"));

            await Assert.ThrowsAsync<AuthorizationException>(() => _postControl.ShowPosts("Org1", "anotheremail@example.com"));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _postControl.ShowPosts("AnotherOrg", "test@example.com"));

            await Assert.ThrowsAsync<UserNotExistInOrganizationException>(() => _postControl.ShowPosts("Org1", "test7@example.com"));
        }
        [Fact]
        public async Task ShowComments()
        {
            Post? post = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com"
                            && p.Organization.Code == "Org1" && p.Text == "ShowPost");
            List<CommentModel> comments = await _postControl.ShowComments("Org1", "test@example.com", post.PostId);
            Assert.True(comments?.Any(c => c.Text == "ShowComment" && c.Email == "test@example.com" && c.Code == "Org1" && c.PostId == post.PostId));

            await Assert.ThrowsAsync<AuthorizationException>(() => _postControl.ShowComments("Org1", "anotheremail@example.com", post.PostId));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _postControl.ShowComments("AnotherOrg", "test@example.com", post.PostId));

            await Assert.ThrowsAsync<UserNotExistInOrganizationException>(() => _postControl.ShowComments("Org1", "test7@example.com", post.PostId));

            await Assert.ThrowsAsync<PostNotExistException>(() => _postControl.ShowComments("Org1", "test@example.com", 100));
        }
        [Fact]
        public async Task CreateComment()
        {
            Post? post = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com"
                            && p.Organization.Code == "Org1" && p.Text == "ShowPost");
            await _postControl.CreateComment("Org1", "test@example.com", post.PostId, "newComment");

            Comment? comment = await _context.Comments
                                .Include(c => c.User)
                                .FirstOrDefaultAsync(c => c.User.Email == "test@example.com"
                                && c.PostId == post.PostId && c.Text == "newComment");

            Assert.NotNull(comment);

            await Assert.ThrowsAsync<AuthorizationException>(() => _postControl.CreateComment("Org1", "anotheremail@example.com", post.PostId, "newComment"));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _postControl.CreateComment("AnotherOrg", "test@example.com", post.PostId, "newComment"));

            await Assert.ThrowsAsync<UserNotExistInOrganizationException>(() => _postControl.CreateComment("Org1", "test7@example.com", post.PostId, "newComment"));

            await Assert.ThrowsAsync<PostNotExistException>(() => _postControl.CreateComment("Org1", "test@example.com", 100, "newComment"));
        }
        [Fact]
        public async Task DeleteComment()
        {
            Post? post = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com"
                            && p.Organization.Code == "Org1" && p.Text == "ShowPost");
            Comment? comment = await _context.Comments
                                .Include(c => c.User)
                                .FirstOrDefaultAsync(c => c.User.Email == "test@example.com"
                                && c.PostId == post.PostId && c.Text == "DeleteComment");
            await _postControl.DeleteComment("Org1", "test@example.com", post.PostId, comment.CommentId);
            Comment? deletedComment = await _context.Comments
                                .Include(c => c.User)
                                .FirstOrDefaultAsync(c => c.User.Email == "test@example.com"
                                && c.PostId == post.PostId && c.Text == "DeleteComment");

            Assert.Null(deletedComment);

            await Assert.ThrowsAsync<AuthorizationException>(() => _postControl.DeleteComment("Org1", "anotheremail@example.com", post.PostId, comment.CommentId));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _postControl.DeleteComment("AnotherOrg", "test@example.com", post.PostId, comment.CommentId));

            await Assert.ThrowsAsync<PostNotExistException>(() => _postControl.DeleteComment("Org1", "test@example.com", 100, comment.CommentId));

            await Assert.ThrowsAsync<CommentNotExistException>(() => _postControl.DeleteComment("Org1", "test@example.com", post.PostId, 100));

            await Assert.ThrowsAsync<RoleAccessException>(() => _postControl.DeleteComment("Org1", "test1@example.com", post.PostId, comment.CommentId));
        }
        [Fact]
        public async Task Like()
        {
            Post? post = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com"
                            && p.Organization.Code == "Org1" && p.Text == "LikePost");
            await _postControl.Like("Org1", "test@example.com", post.PostId);

            Post? likedPost = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .Include(p => p.Likes)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com"
                            && p.Organization.Code == "Org1" && p.Text == "LikePost");

            Assert.True(likedPost.Likes.Count == 1);

            await _postControl.Like("Org1", "test@example.com", post.PostId);

            Assert.True(likedPost.Likes.Count == 1);

            await Assert.ThrowsAsync<AuthorizationException>(() => _postControl.Like("Org1", "anotheremail@example.com", post.PostId));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _postControl.Like("AnotherOrg", "test@example.com", post.PostId));

            await Assert.ThrowsAsync<PostNotExistException>(() => _postControl.Like("Org1", "test@example.com", 100));

            await Assert.ThrowsAsync<UserNotExistInOrganizationException>(() => _postControl.Like("Org1", "test7@example.com", post.PostId));
        }
        [Fact]
        public async Task DisLike()
        {
            Post? post = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com"
                            && p.Organization.Code == "Org1" && p.Text == "DislikePost");
            await _postControl.DisLike("Org1", "test@example.com", post.PostId);

            Post? dislikedPost = await _context.Posts
                            .Include(p => p.User)
                            .Include(p => p.Organization)
                            .Include(p => p.Likes)
                            .FirstOrDefaultAsync(p => p.User.Email == "test@example.com"
                            && p.Organization.Code == "Org1" && p.Text == "DislikePost");

            Assert.True(dislikedPost.Likes.Count == 0);

            await _postControl.DisLike("Org1", "test@example.com", post.PostId);

            Assert.True(dislikedPost.Likes.Count == 0);

            await Assert.ThrowsAsync<AuthorizationException>(() => _postControl.DisLike("Org1", "anotheremail@example.com", post.PostId));

            await Assert.ThrowsAsync<OrganizationNotExistsException>(() => _postControl.DisLike("AnotherOrg", "test@example.com", post.PostId));

            await Assert.ThrowsAsync<PostNotExistException>(() => _postControl.DisLike("Org1", "test@example.com", 100));

            await Assert.ThrowsAsync<UserNotExistInOrganizationException>(() => _postControl.DisLike("Org1", "test7@example.com", post.PostId));
        }
    }
}
