using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<UserOrganization> UserOrganization { get; set; }
        public DbSet<Post>  Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<OrganizationRequest> OrganizationRequests { get; set; }
    }
}
