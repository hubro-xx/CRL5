using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Ocelot.JwtAuthorize
{
    /// <summary>
    /// Ocelot.JwtAuthorize extension
    /// </summary>
    public static class JwtBearerExtension
    {
        /// <summary>
        /// In the Ocelot Project, the Startup. Cs class ConfigureServices method is called
        /// </summary>
        /// <param name="services">Service Collection</param>  
        /// <returns></returns>
        public static AuthenticationBuilder AddOcelotJwtAuthorize(this IServiceCollection services,IConfiguration configuration)
        {
            var config = configuration.GetSection("JwtAuthorize");
            var keyByteArray = Encoding.ASCII.GetBytes(config["Secret"]);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = config["Issuer"],
                ValidateAudience = true,
                ValidAudience = config["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = bool.Parse(config["RequireExpirationTime"])
            };
            return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = bool.Parse(config["IsHttps"]);
                opt.TokenValidationParameters = tokenValidationParameters;
            });
        }

        /// <summary>
        /// In the API Project, the Startup. Cs class ConfigureServices method is called
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="validatePermission">validate permission action</param>
        /// <returns></returns>
        public static void AddApiJwtAuthorize(this IServiceCollection services, IConfiguration configuration, Func<RouteEndpoint, ClaimsPrincipal, bool> validatePermission)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var config = configuration.GetSection("JwtAuthorize");

            var keyByteArray = Encoding.ASCII.GetBytes(config["Secret"]);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = config["Issuer"],
                ValidateAudience = true,
                ValidAudience = config["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = bool.Parse(config["RequireExpirationTime"])
            };
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var permissionRequirement = new JwtAuthorizationRequirement(
                config["Issuer"],
                config["Audience"],
                signingCredentials
                );

            permissionRequirement.ValidatePermission = validatePermission;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
           {
               o.RequireHttpsMetadata = bool.Parse(config["IsHttps"]);
               o.TokenValidationParameters = tokenValidationParameters;
           });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(config["PolicyName"], policy => policy.Requirements.Add(permissionRequirement));

            });
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);

        }
        /// <summary>
        /// In the Authorize Project, the Startup. Cs class ConfigureServices method is called
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <returns></returns>
        public static IServiceCollection AddTokenJwtAuthorize(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("JwtAuthorize");
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["Secret"])), SecurityAlgorithms.HmacSha256);
            var permissionRequirement = new JwtAuthorizationRequirement(
               config["Issuer"],
               config["Audience"],
               signingCredentials
                );
            services.AddSingleton<ITokenBuilder, TokenBuilder>();
            return services.AddSingleton(permissionRequirement);
        }

        /// <summary>
        /// 使用JWT认证调用权限
        /// </summary>
        /// <param name="app"></param>
        public static void UseJwtAuthorization(this IApplicationBuilder app)
        {
            app.UseAuthorization();
            app.UseMiddleware<JwtAuthorizeMiddleware>();
        }
    }
}
