using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Organization;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class UserControl : BaseService, IUserControl
    {
        public UserControl(AppDbContext context) : base(context)
        {

        }
        public async Task<List<OrganizationModel>>  ShowOrganizations(string email)
        {
            User? user = await CheckUserExist(email);

            return await _context.UserOrganizations.Where(uo => uo.UserId == user.UserId)
                .Select(uo => new OrganizationModel() { Name = uo.Organization.Name, Code = uo.Organization.Code })
                .ToListAsync();
        }
        public async Task<UserModel> ShowProfile(string email)
        {
            User? user = await CheckUserExist(email);

            return new UserModel() { Email = user.Email, Name = user.Name, Surname = user.Surname };
        }

    }
}
