using BL.Exceptions;
using BL.Interfaces;
using BL.Models.Organization;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL
{
    public class UserControl : IUserControl
    {
        private readonly AppDbContext _context;
        public UserControl(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<OrganizationModel>>  ShowOrganizations(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
            if (user is null) throw new UserNotExistException();

            return await _context.UserOrganization.Where(uo => uo.UserId == user.UserId)
                .Select(uo => new OrganizationModel() { Name = uo.Organization.Name, Code = uo.Organization.Code })
                .ToListAsync();
        }
        public async Task<UserModel> ShowProfile(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
            if (user is null) throw new UserNotExistException();

            return new UserModel() { Email = user.Email, Name = user.Name, Surname = user.Surname };
        }
    }
}
