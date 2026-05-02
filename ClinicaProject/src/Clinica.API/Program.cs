using Clinica.API.Repositories;
using Clinica.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
 
var builder = WebApplication.CreateBuilder(args);
 
// ── Configurações ────────────────────────────────────────────────────────────
var connStr     = builder.Configuration.GetConnectionString("DefaultConnection")!;
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey   = jwtSettings["SecretKey"]!;
 
// ── Registro de dependências ─────────────────────────────────────────────────
builder.Services.AddSingleton(_ => new AuthRepository(connStr));
builder.Services.AddScoped<IAuthService, AuthService>();
 
// ── JWT Authentication ───────────────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings["Issuer"],
            ValidAudience            = jwtSettings["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
 
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
 
var app = builder.Build();
 
if (app.Environment.IsDevelopment()) app.MapOpenApi();
 
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();