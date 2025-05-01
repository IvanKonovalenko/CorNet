using BL.Models.Post;

namespace BL.Interfaces
{
    public interface IPostControl
    {
        Task CreatePost(string text, string code, string email);
        Task DeletePost(string code, string email, int postId);
        Task<List<PostModel>> ShowPosts(string code, string email);
        Task CreateComment(string code, string email, int postId, string text);
        Task DeleteComment(string code, string email, int postId, int CommentId);
        Task<List<CommentModel>> ShowComments(string code, string email, int postId);
        Task Like(string code, string email, int postId);
        Task DisLike(string code, string email, int postId);
    }
}
