using AcademicPms.Application;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AcademicPms.Application.Imports;
using AcademicPms.Infrastructure.Imports;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PmsDb") ?? throw new InvalidOperationException("Falta configurar ConnectionStrings__PmsDb.");

builder.Services.AddDbContext<PmsDbContext>( options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<ChannelService>();
builder.Services.AddScoped<PublicationService>();
builder.Services.AddScoped<InsideAirbnbCsvReader>();
builder.Services.AddScoped<InsideAirbnbImportService>();
builder.Services.AddScoped<InsideAirbnbCalendarCsvReader>();
builder.Services.AddScoped<InsideAirbnbCalendarImportService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();
