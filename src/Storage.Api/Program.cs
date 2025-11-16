using Storage.Api.DependencyInjection;
using Storage.Api.Hosting;
using Storage.Application.DependencyInjection;
using Storage.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Compose layers
builder.Services.AddApi(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration); // Composition root wires Infrastructure dependencies

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Require internal token for all request to Storage.Api
app.UseMiddleware<InternalAuthMiddleware>();

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { service = "Storage.Api", status = "ok" }));

app.Run();

public partial class Program { }
