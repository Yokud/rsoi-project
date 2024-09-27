using DS_Project.Rentals;
using DS_Project.Rentals.Middleware;
using DS_Project.Rentals.Repository;
using DS_Project.Rentals.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddDbContext<RentalsDbContext>(opt =>
{
    var connectionString = config.GetConnectionString("DefaultConnection");
    opt.UseNpgsql(connectionString, opts => opts.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
});

builder.Services.Configure<JwtConfiguration>(config.GetSection("JwtConfiguration"));
builder.Services.AddScoped<JwtValidator>();

builder.Services.AddScoped<IRentalsRepository, RentalsRepository>();
builder.Services.AddScoped<IRentalsService, RentalsService>();
builder.Services.AddSingleton<LogsProducer>();

builder.Services.AddHealthChecks();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.

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