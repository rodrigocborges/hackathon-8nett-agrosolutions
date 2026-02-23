using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Application.Services;
using AgroSolutions.Identity.Domain.Interfaces;
using AgroSolutions.Identity.Infrastructure.DatabaseContext;
using AgroSolutions.Identity.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura��o do SQLite
var connectionString = builder.Configuration.GetConnectionString("Main") ?? "Data Source=identity.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));

// 2. Inje��o de Depend�ncias
builder.Services.AddScoped<IProducerRepository, ProducerRepository>();
builder.Services.AddScoped<IProducerService, ProducerService>();

// 3. Configura��o do MassTransit (RabbitMQ)
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "amqp://guest:guest@localhost:5672";
        cfg.Host(rabbitHost);
    });
});

// 4. Configura��o do JWT e Autentica��o
var jwtKey = builder.Configuration["Jwt:Key"] ?? "70a297c2-e6f1-45e5-b48c-a303037d3161"; //apenas um placeholder mesmo
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
            ValidateIssuer = false, // Em prod, configurar
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();

// 5. Swagger & AppInsights
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
   options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
});

var app = builder.Build();

// Garantir que o banco de dados SQLite seja criado
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

// --- ENDPOINTS ---
var group = app.MapGroup("/auth");

group.MapPost("/register", async (RegisterRequest request, IProducerService service) =>
{
    try
    {
        var producer = await service.RegisterAsync(request);
        return Results.Ok(new { producer.Id, producer.Name });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

group.MapPost("/login", async (LoginRequest request, IProducerService service) =>
{
    var producer = await service.ValidateLoginAsync(request);
    if (producer == null) return Results.Unauthorized();

    // Gera��o do Token JWT
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(jwtKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, producer.Id.ToString()),
            new Claim(ClaimTypes.Name, producer.Name)
        }),
        Expires = DateTime.UtcNow.AddHours(8),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return Results.Ok(new AuthResponse(producer.Id, producer.Name, tokenHandler.WriteToken(token)));
});

app.Run();