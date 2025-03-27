using BL.Interfaces;
using BL.Models.Post;
using DAL;

namespace BL
{
    public class PostControl : IPostControl
    {
        private readonly AppDbContext _context;
        public PostControl(AppDbContext context)
        {
            _context = context;
        }
        public async Task Create(PostModel model, string code, string email)
        {
            
        }
    }
}
