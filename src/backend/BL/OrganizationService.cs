using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;

public class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _context;
    public OrganizationService(AppDbContext context)
    {
        _context=context;
    }

    public async Task CreateOrganization(OrganizationModel model)
    {
       await ValidateOrganization(model.Code);
       Organization organization=new Organization();
       organization.Code=model.Code;
       organization.Name=model.Name;

        await _context.Organizations.AddAsync(organization);
        await _context.SaveChangesAsync();
    }
    public async Task ValidateOrganization(string code)
    {
        var organization= await _context.Organizations.Where(o=>o.Code==code).FirstOrDefaultAsync();
        if(organization?.OrganizationId ==null)
            return;
        throw new OrganizationException();
    }
    public async Task<List<UserModel>> GetUsers(string email)
    {
        var code = await _context.Users.Where(u=>u.Email==email).Select(u => u.Organization.Code).FirstOrDefaultAsync();;
       return await _context.Users.Where(u=>u.Organization.Code==code&&u.Email!=email).Select(m => new UserModel
        {
            Email = m.Email,
            Name = m.Name,
            Surname = m.Surname,
            Role = m.Role
        })
        .ToListAsync();
    }
}