using BL.Exceptions;
using BL.Models.Auth;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class AuthTest : BaseTest
    {
        [Fact]
        public async Task Register()
        {
            var registerModel = new RegisterModel
            {
                Email = "test9@example.com",
                Password = "password",
                Name = "Test",
                Surname = "User"
            };

            await _auth.Register(registerModel);

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerModel.Email);
            Assert.NotNull(user);

            await Assert.ThrowsAsync<DuplicateEmailException>(() => _auth.Register(registerModel));
        }
        [Fact]
        public async Task Authenticate()
        {
            var authModel = new AuthModel
            {
                Email = "test@example.com",
                Password = "password",
            };
            Assert.NotNull(await _auth.Authenticate(authModel));

            var authModel1 = new AuthModel
            { 
                Email = "test@example.com",
                Password = "anotherpassword",
            };
            await Assert.ThrowsAsync<AuthorizationException>(() => _auth.Authenticate(authModel1));

            var authModel2 = new AuthModel
            {
                Email = "anotheremail@example.com",
                Password = "anotherpassword",
            };
            await Assert.ThrowsAsync<AuthorizationException>(() => _auth.Authenticate(authModel2));
        }

    }
}
