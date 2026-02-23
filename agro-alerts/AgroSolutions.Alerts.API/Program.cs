using AgroSolutions.Alerts.Application.Consumers;
using AgroSolutions.Alerts.Domain.Interfaces;
using AgroSolutions.Alerts.Infrastructure.DatabaseContext;
using AgroSolutions.Alerts.Infrastructure.Repositories;
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
builder.Services.AddScoped<IFieldRepository, FieldRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();

// 3. Configura��o do MassTransit (Consumers)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<FieldCreatedConsumer>();
    x.AddConsumer<SensorDataConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "amqp://guest:guest@localhost:5672";
        cfg.Host(rabbitHost);
        cfg.ConfigureEndpoints(context); // Cria as filas baseadas no nome dos Consumers automaticamente
    });
});

// 4. Configura��o do JWT (Para proteger a leitura dos alertas)
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
var group = app.MapGroup("/alerts").RequireAuthorization();

group.MapGet("/fields/{fieldId:guid}", async (Guid fieldId, IAlertRepository repo) =>
{
    var alerts = await repo.GetByFieldIdAsync(fieldId);
    return Results.Ok(alerts);
});

app.Run();