using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Auth;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;


namespace BL.Services
{
    public class Auth : BaseService, IAuth
    {
        private readonly IEncrypt _encrypt;
        public Auth(AppDbContext context, IEncrypt encrypt) : base(context) 
        {
            _encrypt = encrypt;
        }
        private async Task<User> CreateUser(RegisterModel registerModel)
        {
            var salt = Guid.NewGuid().ToString();    
            var user = new User()
            {
                Salt = salt,
                Password = _encrypt.HashPassword(registerModel.Password, salt),
                Email = registerModel.Email,
                Name = registerModel.Name,
                Surname = registerModel.Surname
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> Authenticate(AuthModel authModel)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(user => user.Email == authModel.Email);

            if (user != null && user.Password == _encrypt.HashPassword(authModel.Password, user.Salt))
            {
                return user;
            }
            throw new AuthorizationException();
        }
        public async Task Register(RegisterModel registerModel)
        {
            await ValidateEmail(registerModel.Email);
            await CreateUser(registerModel);
        }
    }
}
