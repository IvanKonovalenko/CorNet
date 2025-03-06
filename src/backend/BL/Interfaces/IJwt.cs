using DAL.Entities;

namespace BL.Interfaces
{
    public interface IJwt
    {
        string GenerateToken(User user);
    }
}
