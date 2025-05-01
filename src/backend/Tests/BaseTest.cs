using BL;
using BL.Interfaces;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class BaseTest : IDisposable
    {
        protected readonly AppDbContext _context;
        protected readonly IEncrypt _encrypt;
        protected readonly IAuth _auth;
        protected readonly IOrganizationControl _organizationControl;
        protected readonly IPostControl _postControl;
        protected readonly IUserControl _userControl;
        protected BaseTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new AppDbContext(options);
            _encrypt = new Encrypt();
            _auth = new Auth(_context, _encrypt);
            _organizationControl = new OrganizationControl(_context);
            _postControl = new PostControl(_context);
            _userControl = new UserControl(_context);

            CreateTestData();
        }
        public void CreateTestData()
        {
            var salts = new[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
            var users = new[]
            {
                new User { Email = "test@example.com", Password = _encrypt.HashPassword("password", salts[0]), Name = "Test", Surname = "User", Salt = salts[0] },
                new User { Email = "test1@example.com", Password = _encrypt.HashPassword("password", salts[1]), Name = "Test", Surname = "User", Salt = salts[1] },
                new User { Email = "test2@example.com", Password = _encrypt.HashPassword("password", salts[2]), Name = "Test", Surname = "User", Salt = salts[2] },
                new User { Email = "test3@example.com", Password = _encrypt.HashPassword("password", salts[3]), Name = "Test", Surname = "User", Salt = salts[3] },
                new User { Email = "test4@example.com", Password = _encrypt.HashPassword("password", salts[4]), Name = "Test", Surname = "User", Salt = salts[4] },
                new User { Email = "test5@example.com", Password = _encrypt.HashPassword("password", salts[5]), Name = "Test", Surname = "User", Salt = salts[5] },
                new User { Email = "test6@example.com", Password = _encrypt.HashPassword("password", salts[6]), Name = "Test", Surname = "User", Salt = salts[6] },
                new User { Email = "test7@example.com", Password = _encrypt.HashPassword("password", salts[7]), Name = "Test", Surname = "User", Salt = salts[7] },
                new User { Email = "test8@example.com", Password = _encrypt.HashPassword("password", salts[8]), Name = "Test", Surname = "User", Salt = salts[8] }
            };
            var organization = new Organization
            {
                Name = "Organization",
                Code = "Org1"
            };
            var userOrganizations = new[]
            {
                new UserOrganization { Organization = organization, User = users[0], Role = Role.Owner },
                new UserOrganization { Organization = organization, User = users[1], Role = Role.Member },
                new UserOrganization { Organization = organization, User = users[2], Role = Role.Member },
                new UserOrganization { Organization = organization, User = users[3], Role = Role.Member },

            };
            var organizationRequests = new[]
            {
                new OrganizationRequest { Organization = organization, User = users[4],},
                new OrganizationRequest { Organization = organization, User = users[5],},
                new OrganizationRequest { Organization = organization, User = users[6],},
            };
            var posts = new[]
            {
                new Post { Text = "DeletePost", dateTime = DateTime.Now, User = users[0], Organization = organization },
                new Post { Text = "ShowPost", dateTime = DateTime.Now, User = users[0], Organization = organization },
                new Post { Text = "LikePost", dateTime = DateTime.Now, User = users[0], Organization = organization },
                new Post { Text = "DislikePost", dateTime = DateTime.Now, User = users[0], Organization = organization, Likes = new List<User>{ users[0] } }
            };
            var comments = new[]
            {
                new Comment { Text = "ShowComment", dateTime = DateTime.Now, User = users[0], Post = posts[1]},
                new Comment { Text = "DeleteComment", dateTime = DateTime.Now, User = users[0], Post = posts[1]}
            };

            _context.Users.AddRange(users);
            _context.Organizations.Add(organization);
            _context.UserOrganizations.AddRange(userOrganizations);
            _context.OrganizationRequests.AddRange(organizationRequests);
            _context.Posts.AddRange(posts);
            _context.Comments.AddRange(comments);

            _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
