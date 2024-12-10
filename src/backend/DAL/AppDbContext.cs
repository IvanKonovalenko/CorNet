using Microsoft.EntityFrameworkCore;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    { 
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Message> Messages { get; set; }

    
}