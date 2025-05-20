using BL.Exceptions;
using BL.Models.Organization;



namespace Tests
{
    public class UserControlTest : BaseTest
    {
        [Fact]
        public async Task ShowOrganizations()
        {
            List<OrganizationModel> organizations = await _userControl.ShowOrganizations("test@example.com");

            Assert.True(organizations?.Any(o => o.Code == "Org1" && o.Name == "Organization"));

            await Assert.ThrowsAsync<UserNotExistException>(() => _userControl.ShowProfile("anotheremail@example.com"));
        }
        [Fact]
        public async Task ShowProfile()
        {
            UserModel? user = await _userControl.ShowProfile("test@example.com");

            Assert.True(user.Email == "test@example.com" && user.Name == "Test" && user.Surname == "User");

            await Assert.ThrowsAsync<UserNotExistException>(() => _userControl.ShowProfile("anotheremail@example.com"));

        }
    }
}
