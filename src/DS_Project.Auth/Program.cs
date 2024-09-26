using DS_Project.Auth;
using DS_Project.Auth.Config;
using DS_Project.Auth.Repository;
using DS_Project.Auth.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
var app = builder.Build();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddDbContext<UsersDbContext>(opt =>
{
    var connectionString = config.GetConnectionString("DefaultConnection");
    opt.UseNpgsql(connectionString, opts => opts.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
});

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.Configure<JwtConfiguration>(config.GetSection("JwtConfiguration"));
builder.Services.Configure<GatewaySecret>(config.GetSection("GatewaySecret"));

builder.Services.AddHealthChecks();

builder.Services.AddCors();

// Configure the HTTP request pipeline.

app.UseHttpLogging();

app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("X-Total-Count")
                .WithExposedHeaders(""));

app.UseHsts();

app.UseRouting();

app.MapControllers();

app.MapHealthChecks("/manage/health");

app.Run();
