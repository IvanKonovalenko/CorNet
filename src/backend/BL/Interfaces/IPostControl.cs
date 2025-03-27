using BL.Models.Post;

namespace BL.Interfaces
{
    public interface IPostControl
    {
        Task Create(PostModel model, string code, string email);
    }
}
