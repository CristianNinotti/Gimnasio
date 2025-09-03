using Application.Interfaces;
using Application.Mapping;               // ← para AddAutoMapper(UserProfile)
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.ThirstService;     // ← JwtTokenService
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.Extensions;                   // ← AddJwtAuth


var builder = WebApplication.CreateBuilder(args);

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GimnasioDbt")));
builder.Services.AddScoped<DbContext, AppDbContext>();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Auth (JWT + políticas en una sola extensión)
builder.Services.AddJwtAuth(builder.Configuration);

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
app.MapControllers();
app.Run();
