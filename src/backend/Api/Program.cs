var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Services.AddScoped<IAuth,Auth>();
builder.Services.AddScoped<IJwt,Jwt>();
builder.Services.AddScoped<IEncrypt,Encyrpt>();
builder.Services.AddAuth();
builder.Services.AddControllers();


app.UseAuthentication();
app.UseAuthorization();
app.Run();
