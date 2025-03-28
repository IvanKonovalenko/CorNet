using BL.Interfaces;
using BL;
using DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuth, Auth>();
builder.Services.AddScoped<IJwt, Jwt>();
builder.Services.AddScoped<IEncrypt, Encyrpt>();
builder.Services.AddScoped<IOrganizationControl, OrganizationControl>();
builder.Services.AddScoped<IPostControl, PostControl>();
builder.Services.AddScoped<IUserControl, UserControl>();

builder.Services.AddAuth();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetSection("connectionString").Value;
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
