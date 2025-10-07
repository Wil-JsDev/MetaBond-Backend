using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Application.Utils;
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
            services.AddHttpContextAccessor();

            #endregion

            #region Service

            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ICurrentService, CurrentService>();

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
                        var message = context.Exception switch
                        {
                            SecurityTokenExpiredException => JwtMessages.TokenExpired,
                            SecurityTokenInvalidSignatureException => JwtMessages.InvalidSignature,
                            _ => JwtMessages.AuthenticationFailed
                        };

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        var result = JsonConvert.SerializeObject(new JwtResponse(true, message));
                        await context.Response.WriteAsync(result);
                    },

                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        var result = JsonConvert.SerializeObject(new JwtResponse(true, JwtMessages.AccessDenied));
                        await context.Response.WriteAsync(result);
                    },

                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";

                        var result = JsonConvert.SerializeObject(new JwtResponse(true, JwtMessages.Forbidden));
                        await context.Response.WriteAsync(result);
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