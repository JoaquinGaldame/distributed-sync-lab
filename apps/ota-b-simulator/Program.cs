using System.Text.Json.Serialization;
using OtaBSimulator.Application;
using OtaBSimulator.Infrastructure.Http;
using OtaBSimulator.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = ApiValidationResponseFactory.Create;
        options.SuppressMapClientErrors = true;
    });
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler(options => options.ExceptionHandler = _ => Task.CompletedTask);
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddOtaBPersistence(builder.Configuration);
builder.Services.AddOtaBApplication();

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseExceptionHandler();
app.UseStatusCodePages(ApiStatusCodeResponseWriter.WriteAsync);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
