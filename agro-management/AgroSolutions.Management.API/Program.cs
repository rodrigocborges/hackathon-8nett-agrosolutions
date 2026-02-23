using AgroSolutions.Management.Application.Consumers;
using AgroSolutions.Management.Application.DTOs;
using AgroSolutions.Management.Application.Services;
using AgroSolutions.Management.Domain.Interfaces;
using AgroSolutions.Management.Infrastructure.DatabaseContext;
using AgroSolutions.Management.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura��o do SQLite
var connectionString = builder.Configuration.GetConnectionString("Main") ?? "Data Source=database.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));

// 2. Inje��o de Depend�ncias
builder.Services.AddScoped<IProducerRepository, ProducerRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IFieldRepository, FieldRepository>();
builder.Services.AddScoped<IManagementService, ManagementService>();

// 3. Configura��o do MassTransit (RabbitMQ com Consumer)
builder.Services.AddMassTransit(x =>
{
    x.SetLicense("Community");
    
    // Registra o Consumer que escuta a cria��o de produtores
    x.AddConsumer<ProducerRegisteredConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "amqp://guest:guest@localhost:5672";
        cfg.Host(rabbitHost);

        // Configura automaticamente as filas e exchanges para os consumers registrados
        cfg.ConfigureEndpoints(context);
    });
});

// 4. Configura��o do JWT (Mesma chave do Identity)
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

//builder.Services.AddApplicationInsightsTelemetry(options =>
//{
//    options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
//});

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

// --- ENDPOINTS PROTEGIDOS (Requerem Token JWT) ---
var group = app.MapGroup("/management").RequireAuthorization();

// Helper para pegar o ID do produtor pelo Token JWT
Guid GetProducerId(HttpContext context)
{
    var claim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
}

group.MapPost("/properties", async (CreatePropertyRequest request, IManagementService service, HttpContext httpContext) =>
{
    var producerId = GetProducerId(httpContext);
    var property = await service.CreatePropertyAsync(request, producerId);
    return Results.Ok(property);
});

group.MapGet("/properties", async (IPropertyRepository repo, HttpContext httpContext) =>
{
    var producerId = GetProducerId(httpContext);
    var properties = await repo.GetAllByProducerIdAsync(producerId);
    return Results.Ok(properties);
});

group.MapPost("/fields", async (CreateFieldRequest request, IManagementService service, HttpContext httpContext) =>
{
    try
    {
        var producerId = GetProducerId(httpContext);
        var field = await service.CreateFieldAsync(request, producerId);
        return Results.Ok(field);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Results.Forbid();
    }
});

group.MapGet("/properties/{propertyId:guid}/fields", async (Guid propertyId, IFieldRepository repo) =>
{
    var fields = await repo.GetAllByPropertyIdAsync(propertyId);
    return Results.Ok(fields);
});

app.Run();