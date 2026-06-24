using System.Text;
using Application.Abstractions;
using Application.Authentication.Interfaces;
using Application.Common.Configurations;
using Application.Common.Interfaces;
using Application.Common.Persistence;
using Application.TaskItems.Interfaces;
using EasyNetQ;
using Infrastructure.Authentication;
using Infrastructure.Common;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.TaskItems;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        services.AddSingleton<ITaskItemExportXlsx, TaskItemExportXlsx>();
        services.AddSingleton<ITaskItemExportPdf, TaskItemExportPdf>();
        QuestPDF.Settings.License = LicenseType.Community;
        services.AddSingleton<ITaskItemExportStrategyFactory, TaskItemExportStrategyFactory>();
        services.AddAuth(configuration);
        services.AddPersistence(configuration);
        services.AddAsyncMessageBus(configuration);
        services.AddEmailSender(configuration);

        return services;
    }
    
    private static IServiceCollection AddAuth(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var jwtSettings = configuration.GetSection(key: nameof(JwtOptions));
        var jwtOptions = jwtSettings.Get<JwtOptions>()!;
        services.Configure<JwtOptions>(jwtSettings);
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                };
            });

        return services;
    }
    
    private static IServiceCollection AddPersistence(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddDbContext<AppDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString(name: "DefaultConnection")));

        return services;
    }
    
    private static IServiceCollection AddEmailSender(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<EmailSenderOptions>(configuration.GetSection(key: nameof(EmailSenderOptions)));
        services.AddSingleton<IEmailSender, EmailSender>();

        return services;
    }
    
    private static IServiceCollection AddAsyncMessageBus(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddEasyNetQ(configuration.GetConnectionString(name: "AsyncBusConnection")).UseSystemTextJson();
        services.AddSingleton<IMessageBus, RabbitMqBus>();
        
        return services;
    }
}
