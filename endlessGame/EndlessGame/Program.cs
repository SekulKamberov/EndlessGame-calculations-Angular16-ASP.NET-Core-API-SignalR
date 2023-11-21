using EndlessGame.Entities;
using EndlessGame.HubConfig;
using Microsoft.EntityFrameworkCore;
 
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
  options.AddPolicy("CorsPolicy", builder => builder
      .WithOrigins("http://localhost:4200")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials());
});

builder.Services.AddSignalR();

 
//builder.Services.AddDbContext<EndlessGameDBContext>(opt => opt.UseSqlServer("DevConnection"));

builder.Services.AddControllers();
builder.Services.AddDbContext<EndlessGameDBContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();
