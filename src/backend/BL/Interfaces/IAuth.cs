using BL.Models;
using DAL.Entities;

namespace BL.Interfaces
{
    public interface IAuth
    {
        Task<User> Authenticate(AuthModel authModel);
        Task Register(RegisterModel registerModel);
    }
}
