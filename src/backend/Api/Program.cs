using BL.Interfaces;
using BL;
using DAL;
using Microsoft.EntityFrameworkCore;
using Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuth, Auth>();
builder.Services.AddScoped<IJwt, Jwt>();
builder.Services.AddScoped<IEncrypt, Encrypt>();
builder.Services.AddScoped<IOrganizationControl, OrganizationControl>();
builder.Services.AddScoped<IPostControl, PostControl>();
builder.Services.AddScoped<IUserControl, UserControl>();

builder.Services.AddAuth();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetSection("connectionString").Value;
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    await TestData.CreateTestData(dbContext);
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
