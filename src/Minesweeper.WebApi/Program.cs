using Minesweeper.Application;
using Minesweeper.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application services
builder.Services.AddApplicationServices();

// Add Infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minesweeper API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
    app.UseCors("Development");
}

app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow })
    .WithName("HealthCheck")
    .WithTags("Health");

// Minesweeper game info endpoint
app.MapGet("/api/info", () => new
{
    Name = "Minesweeper API",
    Version = "1.0.0",
    Description = "Enterprise Minesweeper Game API with Clean Architecture",
    Features = new[] { "Game Management", "Real-time Updates", "Statistics", "Multiple Difficulties" }
})
.WithName("GetGameInfo")
.WithTags("Game");

app.Run();
