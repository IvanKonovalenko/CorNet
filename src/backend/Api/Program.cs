var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddAuth();
builder.Services.AddControllers();

app.MapGet("/", () => "Hello World!");


app.UseAuthentication();
app.UseAuthorization();
app.Run();
