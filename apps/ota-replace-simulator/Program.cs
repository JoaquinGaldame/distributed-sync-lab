using Microsoft.EntityFrameworkCore;
using OtaReplaceSimulator.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OtaReplaceSimulatorDb")
    ?? throw new InvalidOperationException("Falta configurar ConnectionStrings__OtaReplaceSimulatorDb.");

builder.Services.AddDbContext<SimulatorDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
