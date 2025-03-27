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
        public async Task<User> CreateUser(RegisterModel registerModel)
        {
            User user = new User();
            user.Salt = Guid.NewGuid().ToString();
            user.Password = _encrypt.HashPassword(registerModel.Password, user.Salt);
            user.Email = registerModel.Email;
            user.Name = registerModel.Name;
            user.Surname = registerModel.Surname;



            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> Authenticate(AuthModel authModel)
        {
            User? user = await _context.Users.Where(user => user.Email == authModel.Email).FirstOrDefaultAsync();

            if (user?.Password == _encrypt.HashPassword(authModel.Password, user.Salt))
            {
                return user;
            }
            throw new AuthorizationException();
        }
        public async Task ValidateEmail(string email)
        {
            User? user = await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (user?.UserId != null)
                throw new DuplicateEmailException();
        }

        public async Task Register(RegisterModel registerModel)
        {
            await ValidateEmail(registerModel.Email);
            await CreateUser(registerModel);
        }
    }
}
