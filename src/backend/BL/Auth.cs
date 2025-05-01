using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Auth;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;


namespace BL
{
    public class Auth : IAuth
    {
        private readonly AppDbContext _context;
        private readonly IEncrypt _encrypt;
        public Auth(AppDbContext context, IEncrypt encrypt)
        {
            _context = context;
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
        private async Task ValidateEmail(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
            if (user != null)
                throw new DuplicateEmailException();
        }

        public async Task Register(RegisterModel registerModel)
        {
            await ValidateEmail(registerModel.Email);
            await CreateUser(registerModel);
        }
    }
}
