using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Likes)
                .WithMany(u => u.Likes)
                .UsingEntity(j => j.ToTable("PostLikes"));

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)           
                .WithMany(u => u.Posts)        
                .HasForeignKey(p => p.UserId);


            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<Post>  Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<OrganizationRequest> OrganizationRequests { get; set; }
    }
}
