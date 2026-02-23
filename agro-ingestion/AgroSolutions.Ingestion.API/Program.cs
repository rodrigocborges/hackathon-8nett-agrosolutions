using AgroSolutions.Ingestion.Application.DTOs;
using AgroSolutions.Ingestion.Application.Services;
using AgroSolutions.Ingestion.Domain.Interfaces;
using AgroSolutions.Ingestion.Infrastructure.DatabaseContext;
using AgroSolutions.Ingestion.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura��o do SQLite
var connectionString = builder.Configuration.GetConnectionString("Main") ?? "Data Source=database.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));

// 2. Inje��o de Depend�ncias
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<IIngestionService, IngestionService>();

// 3. Configura��o do MassTransit (Apenas Publisher, n�o consome nada)
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "amqp://guest:guest@localhost:5672";
        cfg.Host(rabbitHost);
    });
});

// 4. Configura��o do JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "70a297c2-e6f1-45e5-b48c-a303037d3161";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
   options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.GetPendingMigrations().Any()) db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

// --- ENDPOINTS ---
var group = app.MapGroup("/ingestion").RequireAuthorization(); // Exig�ncia do MVP: API Autenticada

group.MapPost("/sensor-data", async (SensorDataRequest request, IIngestionService service) =>
{
    if (request.FieldId == Guid.Empty)
        return Results.BadRequest("FieldId inv�lido.");

    await service.ProcessSensorDataAsync(request);

    // 202 Accepted: recebido, mas o processamento final (alertas) ser� ass�ncrono.
    return Results.Accepted();
});

app.Run();