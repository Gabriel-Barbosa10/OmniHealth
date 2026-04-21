using System.Text;
using Clinica.API.Models;
using Clinica.API.Repositories;
using Clinica.API.Services;

var builder = WebApplication.CreateBuilder(args);

// 4. Controllers + Swagger
// builder.Services.AddAuthorization();
// builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // ← deve vir antes do Authorization
app.UseAuthorization();
app.MapControllers();

app.Run();


// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;

// 1. Configurar SQLite
// builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlite(builder.Configuration.GetConnectionString("ClinicaDb")));

// 2. Repositório e serviço de autenticação
// builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
// builder.Services.AddScoped<IAuthService, AuthService>();

// 3. JWT
// var jwtSection = builder.Configuration.GetSection("Jwt");
// var secretKey  = jwtSection["SecretKey"]!;

// builder.Services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer           = true,
//             ValidateAudience         = true,
//             ValidateLifetime         = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer              = jwtSection["Issuer"],
//             ValidAudience            = jwtSection["Audience"],
//             IssuerSigningKey         = new SymmetricSecurityKey(
//                                            Encoding.UTF8.GetBytes(secretKey))
//         };
//     });

