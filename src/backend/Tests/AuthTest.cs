using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Moq;

public class AuthTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IEncrypt> _encryptMock;

    public AuthTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new AppDbContext(options);
        _encryptMock = new Mock<IEncrypt>();
    }

    [Fact]
    public async Task ValidateEmail_ThrowsDuplicateEmailException_WhenEmailExists()
    {
        // Arrange
        var auth = new Auth(_context, _encryptMock.Object);
        await _context.Users.AddAsync(new User { Email = "test@example.com",
        Name = "John",
        Surname = "Doe",
        Password = "hashedpassword",
        Salt = "randomSalt",
        Role = Role.Employee,
        Organization = new Organization { Name = "Test Org", Code = "ORG001" } });
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEmailException>(() => auth.ValidateEmail("test@example.com"));
    }

    [Fact]
    public async Task ValidateOrganization_ReturnsOrganization_WhenCodeExists()
    {
        // Arrange
        var auth = new Auth(_context, _encryptMock.Object);
        var organization = new Organization { Code = "ORG001", Name = "Test Org" };
        await _context.Organizations.AddAsync(organization);
        await _context.SaveChangesAsync();

        // Act
        var result = await auth.ValidateOrganization("ORG001");

        // Assert
        Assert.Equal("Test Org", result.Name);
    }

    [Fact]
    public async Task CreateUser_CreatesUserSuccessfully()
    {
        // Arrange
        var auth = new Auth(_context, _encryptMock.Object);
        var organization = new Organization { Code = "ORG001", Name = "Test Org" };
        await _context.Organizations.AddAsync(organization);
        await _context.SaveChangesAsync();

        var authModel = new AuthModel
        {
            Email = "test@example.com",
            Name = "John",
            Surname = "Doe",
            Password = "password123",
            OrganizationCode = "ORG001",
            Role = Role.Employee
        };

        _encryptMock.Setup(e => e.HashPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("hashedpassword");

        // Act
        var user = await auth.CreateUser(authModel);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("John", user.Name);
        Assert.Equal("Doe", user.Surname);
        Assert.Equal("hashedpassword", user.Password);
    }

    [Fact]
    public async Task Authenticate_ThrowsAuthorizationException_WhenInvalidCredentials()
    {
        // Arrange
        var auth = new Auth(_context, _encryptMock.Object);
        var user = new User
        {
            Email = "test@example.com",
            Name = "John",
             Surname = "Doe",
            Password = "hashedpassword",
            Salt = "randomSalt",
            Role = Role.Employee,
            Organization = new Organization { Name = "Test Org", Code = "ORG001" }
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        _encryptMock.Setup(e => e.HashPassword("wrongpassword", "salt123")).Returns("wronghash");

        // Act & Assert
        await Assert.ThrowsAsync<AuthorizationExeception>(() => auth.Authenticate("test@example.com", "wrongpassword"));
    }
}
