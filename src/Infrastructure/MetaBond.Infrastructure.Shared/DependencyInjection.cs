using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Application.Utils;
using MetaBond.Domain.Settings;
using MetaBond.Infrastructure.Shared.Service;
using MetaBond.Infrastructure.Shared.SignaIR.Senders;
using Microsoft.AspNetCore.Authentication;
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
            services.AddScoped<IGitHubAuthService, GitHubAuthService>();

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
                })
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/api/v1/auth/github-login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddOAuth("GitHub", options =>
                {
                    var githubConfig = configuration.GetSection("GitHubAuthentication");

                    var clientId = githubConfig["ClientId"];
                    var clientSecret = githubConfig["ClientSecret"];

                    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                    {
                        throw new InvalidOperationException(
                            "GitHub OAuth configuration is missing. Please ensure ClientId and ClientSecret are set in GitHubAuthentication section."
                        );
                    }

                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;

                    options.CallbackPath = "/signin-github";

                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";

                    options.Scope.Add("user:email");
                    options.Scope.Add("read:user");

                    options.ClaimActions.MapJsonKey("urn:github:id", "id");
                    options.ClaimActions.MapJsonKey("urn:github:login", "login");
                    options.ClaimActions.MapJsonKey("urn:github:name", "name");
                    options.ClaimActions.MapJsonKey("urn:github:email", "email");
                    options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

                    options.SignInScheme = "Cookies";

                    options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            // Step 1: Get the user's information from GitHub.
                            var request =
                                new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);

                            request.Headers.Accept.Add(
                                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            request.Headers.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                            // Send the request and get the response.
                            var response = await context.Backchannel.SendAsync(request,
                                HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);

                            response.EnsureSuccessStatusCode();

                            var user = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                            context.RunClaimActions(user.RootElement);

                            // Step 2: Get the user's email address.'
                            if (!context.Identity.HasClaim(c => c.Type == "urn:github:email"))
                            {
                                var emailRequest =
                                    new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");

                                emailRequest.Headers.Accept.Add(
                                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                emailRequest.Headers.Authorization =
                                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                                        context.AccessToken);

                                var emailResponse = await context.Backchannel.SendAsync(emailRequest,
                                    HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);

                                if (emailResponse.IsSuccessStatusCode)
                                {
                                    var emails =
                                        System.Text.Json.JsonDocument.Parse(await emailResponse.Content
                                            .ReadAsStringAsync());

                                    // Get the primary email address.
                                    var primaryEmail = emails.RootElement.EnumerateArray()
                                        .FirstOrDefault(e => e.GetProperty("primary").GetBoolean());

                                    if (primaryEmail.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                                    {
                                        var email = primaryEmail.GetProperty("email").GetString();
                                        if (!string.IsNullOrEmpty(email))
                                        {
                                            // Add the email address to the user's claims.
                                            context.Identity.AddClaim(
                                                new System.Security.Claims.Claim("urn:github:email", email));
                                        }
                                    }
                                }
                            }
                        }
                    };
                });

            #endregion

            #region SignaIR

            services.AddSignalR();
            services.AddTransient<INotificationSender, NotificationSender>();
            services.AddTransient<IChatSender, ChatSender>();

            #endregion
        }
    }
}