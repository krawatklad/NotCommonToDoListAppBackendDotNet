using Api.Common.Exceptions;
using Api.Common.OpenApi;
using Microsoft.OpenApi.Models;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddDocumentation();
        services.AddGlobalExceptionHandling();
        services.AddAuthorization();
        
        return services;
    }
    
    private static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        services.AddSingleton<IProblemDetailsFactory, ProblemDetailsFactory>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        return services;
    }
    
    private static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "NotCommonToDoListAppBackend API",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization"
            });

            options.OperationFilter<AuthorizeOperationFilter>();
        });

        return services;
    }
}