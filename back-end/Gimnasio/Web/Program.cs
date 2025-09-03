using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.ThirstService;     // ← JwtTokenService
using Microsoft.EntityFrameworkCore;
using Web.Extensions;                   // ← AddJwtAuth


var builder = WebApplication.CreateBuilder(args);

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAppSwagger(); // ← 1) agrega Swagger + botón Authorize

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GimnasioDbt")));
builder.Services.AddScoped<DbContext, AppDbContext>();

// Auth (JWT + políticas en una sola extensión)
builder.Services.AddJwtAuth(builder.Configuration);

// Aca

// Servicios
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>(); // servicio de terceros
builder.Services.AddScoped<IUserService, UserService>();

// Repos + UoW
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseAppSwagger();                        // ← muestra Swagger UI
app.MapControllers();
app.Run();
