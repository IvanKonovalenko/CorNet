using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAuth,Auth>();
builder.Services.AddScoped<IJwt,Jwt>();
builder.Services.AddScoped<IEncrypt,Encyrpt>();
builder.Services.AddScoped<IMessagesService,MessagesService>();
builder.Services.AddScoped<IOrganizationService,OrganizationService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("User ID=postgres; Password=password; Host=localhost; Port=5432; Database=cornet;"));

builder.Services.AddAuth();
builder.Services.AddControllers();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated(); 
}


app.UseCors(builder => builder.AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
