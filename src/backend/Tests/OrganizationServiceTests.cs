using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Moq;
public class OrganizationServiceTests
{
    private readonly AppDbContext _context;

    public OrganizationServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrganization_CreatesOrganizationSuccessfully()
    {
        // Arrange
        var service = new OrganizationService(_context);
        var model = new OrganizationModel { Name = "Org Test", Code = "CODE123" };

        // Act
        await service.CreateOrganization(model);

        // Assert
        var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Code == "CODE123");
        Assert.NotNull(organization);
        Assert.Equal("Org Test", organization.Name);
    }

}
