using DAL.Entities;

namespace BL.Models.Organization
{
    public class UserModel
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Role { get; set; } = null!;

    }
}
