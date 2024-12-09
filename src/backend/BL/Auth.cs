using Microsoft.EntityFrameworkCore;

public class Auth : IAuth
{
    private readonly AppDbContext _context;
    private readonly IEncrypt _encrypt;
    public Auth(AppDbContext context, IEncrypt encrypt)
    {
        _context=context;
        _encrypt=encrypt;
    }

    public async Task<User> CreateUser(AuthModel model)
    {
        var organization=await ValidateOrganization(model.OrganizationCode);
        User user= new User();
        user.Salt=Guid.NewGuid().ToString();
        user.Password=_encrypt.HashPassword(model.Password,user.Salt);
        user.Email=model.Email;
        user.Name=model.Name;
        user.Surname=model.Surname;
        user.Organization=organization;
        user.Role=model.Role;

        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
   
    public async Task<User> Authenticate(string email, string password)
    {
        var user= await _context.Users.Where(user=>user.Email==email).FirstOrDefaultAsync(); 

        if(user?.UserId != null && user?.Password == _encrypt.HashPassword(password,user.Salt))
        {
            return user;
        }
        throw new AuthorizationExeception();
    }
    public async Task ValidateEmail(string email)
    {
        var user= await _context.Users.Where(user=>user.Email==email).FirstOrDefaultAsync();
        if(user?.UserId !=null)
            throw new DuplicateEmailException();
    }
    public async Task<Organization> ValidateOrganization(string code)
    {
        var organization= await _context.Organizations.Where(o=>o.Code==code).FirstOrDefaultAsync();
        if(organization?.OrganizationId !=null)
            return organization;
        throw new OrganizationException();
    }

    public async Task Register(AuthModel model)
    {
        await ValidateEmail(model.Email);
        await CreateUser(model);

    }
}