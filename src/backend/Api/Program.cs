using BL.Interfaces;
using DAL;
using Microsoft.EntityFrameworkCore;
using Api;
using BL.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuth, Auth>();
builder.Services.AddScoped<IJwt, Jwt>();
builder.Services.AddScoped<IEncrypt, Encrypt>();
builder.Services.AddScoped<IOrganizationControl, OrganizationControl>();
builder.Services.AddScoped<IPostControl, PostControl>();
builder.Services.AddScoped<IUserControl, UserControl>();
builder.Services.AddScoped<IMessageControl, MessageControl>();
builder.Services.AddSignalR();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetSection("connectionString").Value;
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var port = builder.Configuration.GetSection("port").Value;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins($"http://localhost:{port}")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    var testData = builder.Configuration.GetSection("createTestData").Value;
    if (Convert.ToBoolean(testData))
    {
        await TestData.CreateTestData(dbContext);
    }
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub").RequireCors("AllowFrontend");

app.Run();
