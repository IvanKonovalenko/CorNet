using BL.Models.Auth;
using DAL.Entities;

namespace BL.Interfaces
{
    public interface IAuth
    {
        Task<User> Authenticate(AuthModel authModel);
        Task Register(RegisterModel registerModel);
    }
}
