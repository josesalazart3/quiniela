using Microsoft.EntityFrameworkCore;
using Quiniela.Data;
using System.Text;
using Quiniela.Models;
using Quiniela.Utils;
using Quiniela.Repositories;
using Quiniela.Repositories.Interfaces;
using Quiniela.Services;
using Quiniela.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Quiniela.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Quiniela.Hubs;

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
    string key = "62219311522870687600240042448129"; // 32 chars
    string iv = "8458586964174710";                  // 16 chars
    return new CryptoHelper(key, iv);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("quiniela"));


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));


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
});

builder.Services.AddAuthorization();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//apply DB migrations and seed data
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