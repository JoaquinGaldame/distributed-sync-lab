using OtaASimulator.Application;
using OtaASimulator.Infrastructure.Http;
using OtaASimulator.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = ApiValidationResponseFactory.Create;
        options.SuppressMapClientErrors = true;
    });
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler(options =>
    options.ExceptionHandler = _ => Task.CompletedTask);
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddOtaAPersistence(builder.Configuration);
builder.Services.AddOtaAApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseStatusCodePages(ApiStatusCodeResponseWriter.WriteAsync);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
