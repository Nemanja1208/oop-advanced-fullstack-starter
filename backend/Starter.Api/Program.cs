using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Starter.Api.Data;
using Starter.Api.Features.Todos;

const string FrontendCorsPolicy = "frontend";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<TodoService>();

// The customer works in Swedish time, wherever the server runs (kravspec: "Svensk tid, alltid").
var deadlineTimeZone = TimeZoneInfo.FindSystemTimeZoneById(builder.Configuration["Deadlines:TimeZone"] ?? "Europe/Stockholm");
builder.Services.AddSingleton(sp => new DeadlineCalendar(sp.GetRequiredService<TimeProvider>(), deadlineTimeZone));

var frontendOrigin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
    options.AddPolicy(FrontendCorsPolicy, policy => policy.WithOrigins(frontendOrigin).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Good enough for a starter. A real project uses EF Core migrations instead.
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseCors(FrontendCorsPolicy);
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

/// <summary>Exposed so integration tests can start the app with WebApplicationFactory.</summary>
public partial class Program;
