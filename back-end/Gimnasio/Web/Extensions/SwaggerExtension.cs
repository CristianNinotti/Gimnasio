using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Web.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddAppSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Gimnasio API",
                Version = "v1"
            });

            // 🔐 Definición de esquema Bearer (esto habilita el botón Authorize)
            var jwtScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Pegá SOLO el token (sin 'Bearer '). Swagger lo agrega.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // minúsculas
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            c.AddSecurityDefinition("Bearer", jwtScheme);

            // Requisito global: envía Authorization: Bearer <token>
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtScheme, Array.Empty<string>() }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseAppSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gimnasio API v1");
            c.RoutePrefix = "swagger"; // https://localhost:xxxx/swagger
        });
        return app;
    }
}
