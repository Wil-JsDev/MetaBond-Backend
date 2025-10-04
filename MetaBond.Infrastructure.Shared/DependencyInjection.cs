using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Domain.Settings;
using MetaBond.Infrastructure.Shared.Service;
using MetaBond.Infrastructure.Shared.SignaIR.Senders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace MetaBond.Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static void AddShared(this IServiceCollection services, IConfiguration configuration)
        {
            #region Configuration

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            #endregion

            #region Service

            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtService, JwtService>();

            #endregion

            #region Jwt

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                // For production, set RequireHttpsMetadata to true
                options.RequireHttpsMetadata = true; // Change to true for better security
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Convert.FromBase64String(configuration["JwtSettings:Key"] ?? string.Empty)),
                    // Explicitly validate the algorithm
                    RequireSignedTokens = true,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async context =>
                    {
                        // Use generic error messages to avoid leaking details
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new JwtResponse(true, "Authentication failed."));
                        await context.Response.WriteAsync(result);
                    },

                    OnChallenge = async c =>
                    {
                        c.HandleResponse();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new JwtResponse(true, "Access denied."));
                        await c.Response.WriteAsync(result);
                    },

                    OnForbidden = async c =>
                    {
                        c.Response.StatusCode = 403;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new JwtResponse(true,
                            "You do not have permission to access this resource."));
                        await c.Response.WriteAsync(result);
                    },

                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            #endregion
            
            #region SignaIR
            
            services.AddSignalR();
            services.AddTransient<INotificationSender, NotificationSender>();
            
            #endregion
        }
    }
}