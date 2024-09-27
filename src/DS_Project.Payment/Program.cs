using DS_Project.Payments;
using DS_Project.Payments.Repository;
using DS_Project.Payments.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddDbContext<PaymentsDbContext>(opt =>
{
    var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
    var connectionString = config.GetConnectionString("DefaultConnection");
    opt.UseNpgsql(connectionString, opts => opts.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
});

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
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