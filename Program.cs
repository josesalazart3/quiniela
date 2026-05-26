using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quiniela.Data;
using Quiniela.Hubs;
using Quiniela.Middlewares;
using Quiniela.Models;
using Quiniela.Repositories;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services;
using Quiniela.Services.Interfaces;
using Quiniela.Utils;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(new { error = string.Join(", ", errors) });
        };
    });

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var key = config["Crypto:Key"] ?? throw new InvalidOperationException("Crypto:Key no configurada");
    var iv = config["Crypto:IV"] ?? throw new InvalidOperationException("Crypto:IV no configurada");
    return new CryptoHelper(key, iv);
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
  //                     ?? builder.Configuration["ConnectionStrings__DefaultConnection"]
    //                   ?? builder.Configuration["DATABASE_URL"];

//Console.WriteLine($"DEBUG: La cadena de conexión leída es: '{connectionString}'");

//builder.Services.AddDbContext<AppDbContext>(options =>
    //options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))*/
  //  options.UseNpgsql(builder.Configuration["ConnectionStrings__DefaultConnection"])
    //       .LogTo(Console.WriteLine, LogLevel.Information)
      //     .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Leer la cadena de conexión de diferentes fuentes
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                           ?? builder.Configuration["ConnectionStrings__DefaultConnection"]
                           ?? builder.Configuration["DATABASE_URL"];
    
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("No se encontró ninguna cadena de conexión configurada");
    }
    
    Console.WriteLine($"DEBUG: Usando cadena de conexión: '{connectionString}'");
    
    // Si la cadena es una URL de PostgreSQL, convertirla al formato estándar
    if (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://"))
    {
        var uri = new Uri(connectionString);
        var username = uri.UserInfo.Split(':')[0];
        var password = uri.UserInfo.Split(':')[1];
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        
        connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
        
        Console.WriteLine($"DEBUG: Cadena convertida a: '{connectionString}'");
    }

    options.UseNpgsql(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});




builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ITorneoRepository, TorneoRepository>();
builder.Services.AddScoped<ITorneoService, TorneoService>();

builder.Services.AddScoped<IEstadioRepository, EstadioRepository>();
builder.Services.AddScoped<IEstadioService, EstadioService>();

builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
builder.Services.AddScoped<IEquipoService, EquipoService>();

builder.Services.AddScoped<IFaseRepository, FaseRepository>();
builder.Services.AddScoped<IFaseService, FaseService>();

builder.Services.AddScoped<IGrupoRepository, GrupoRepository>();
builder.Services.AddScoped<IGrupoService, GrupoService>();

builder.Services.AddScoped<IClasificacionGrupoRepository, ClasificacionGrupoRepository>();

builder.Services.AddScoped<IPartidoRepository, PartidoRepository>();
builder.Services.AddScoped<IPartidoService, PartidoService>();

builder.Services.AddScoped<IPrediccionRepository, PrediccionRepository>();
builder.Services.AddScoped<IPrediccionService, PrediccionService>();

builder.Services.AddScoped<ILigaRepository, LigaRepository>();
builder.Services.AddScoped<ILigaMiembroRepository, LigaMiembroRepository>();
builder.Services.AddScoped<ILigaService, LigaService>();

builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IInvitacionLigaRepository, InvitacionLigaRepository>();
builder.Services.AddScoped<IInvitacionLigaService, InvitacionLigaService>();

builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

builder.Services.AddScoped<IPremioDistribuidoRepository, PremioDistribuidoRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IRankingService, RankingService>();
builder.Services.AddScoped<BracketService>();


builder.Services.AddSignalR();
builder.Services.AddScoped<INotificacionService, NotificacionService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no configurada");
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs/quiniela"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "http://localhost:4300",
                "https://localhost:4300",
                "http://localhost"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply DB migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DatosMundial.SeedAsync(db);
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();
app.MapHub<QuinielaHub>("/hubs/quiniela");

app.Run();