using System;
using System.Text;
using AuthenticationEmbedder.Authentication;
using AuthenticationEmbedder.Repository;
using InfoLog;
using InfoLog.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationEmbedder;

public static class ServicesExtension
{
    public static void AddLogger<T>(this IServiceCollection services, string xmlPath)
        where T : ILogger, new()
    {
        services.AddSingleton(provider =>
        {
            var configuration = new Configuration(xmlPath);
            var loggerFactory = new LoggerFactory<T>(configuration);
            return loggerFactory.CreateLogger();
        });
    }

    public static void AddJwtSigningAuthentication<T>(this IServiceCollection services, string securityKey) 
        where T: IJwtSigningEncodingKey, new()
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        T jwtSigning = new T { SecretKey = symmetricSecurityKey };
        services.AddSingleton<IJwtSigningEncodingKey>(jwtSigning);
            
        const string jwtSchemeName = "JwtBearer";
        services
            .AddAuthentication(options => {
                options.DefaultAuthenticateScheme = jwtSchemeName;
                options.DefaultChallengeScheme = jwtSchemeName;
            })
            .AddJwtBearer(jwtSchemeName, jwtBearerOptions => {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtSigning.GetKey(),
 
                    ValidateIssuer = true,
                    ValidIssuer = "Authentication",
 
                    ValidateAudience = true,
                    ValidAudience = "AuthenticationClient",
 
                    ValidateLifetime = true,
 
                    ClockSkew = TimeSpan.FromSeconds(5)
                };
            });
    }

    public static void AddDataRequest<T>(this IServiceCollection services) 
        where T : IRepository, new ()
    {
        services.AddScoped<IRepository>(provider =>
        {
            DataContext dataContext = provider.GetService<DataContext>();
            T databaseRequest = new T { Context = dataContext };
            return databaseRequest;
        });
    }
}