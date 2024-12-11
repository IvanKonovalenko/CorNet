using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов
builder.Services.AddScoped<IAuth, Auth>();
builder.Services.AddScoped<IJwt, Jwt>();
builder.Services.AddScoped<IEncrypt, Encyrpt>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("User ID=postgres; Password=password; Host=localhost; Port=5432; Database=cornet;"));

builder.Services.AddAuth();
builder.Services.AddControllers();

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Убедитесь, что база данных создаётся при запуске
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Промежуточное ПО
app.UseCors(); // Подключаем настройку CORS
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
