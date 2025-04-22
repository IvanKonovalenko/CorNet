using BL.Models.Post;

namespace BL.Interfaces
{
    public interface IPostControl
    {
        Task Create(CreatePostModel model, string code, string email);
        Task<List<PostModel>> ShowPosts(string code, string email);
        Task<List<CommentModel>> ShowComments(string code, string email, int postId);
    }
}
