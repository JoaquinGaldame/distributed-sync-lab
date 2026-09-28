using AcademicPms.Application;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PmsDb") ?? throw new InvalidOperationException("Falta configurar ConnectionStrings__PmsDb.");

builder.Services.AddDbContext<PmsDbContext>( options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<PropertyService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();
