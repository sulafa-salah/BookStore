using Cart.Application.Common.Interfaces;
using Cart.Infrastructure.Authentication.TokenSetting;
using Cart.Infrastructure.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Infrastructure;
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
             services.AddJWTAuthentication(configuration)
       
         .AddConfigurations(configuration);
            return services;
        }
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<RedisOptions>(cfg.GetSection("Redis"));
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var cs = cfg["Redis:ConnectionString"] ?? cfg["ConnectionStrings:Redis"] ?? "redis:6379";
            return ConnectionMultiplexer.Connect(cs);
        });
        services.AddSingleton<ICartRepository, RedisCartRepository>();
        return services;
    }
    public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.Section, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {


                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero,
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx => { Console.WriteLine(ctx.Exception); return Task.CompletedTask; },
                    OnChallenge = ctx => { Console.WriteLine($"Challenge: {ctx.Error} {ctx.ErrorDescription}"); return Task.CompletedTask; }
                };
            });


        return services;
    }
}