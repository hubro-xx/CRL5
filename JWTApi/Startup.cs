using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.JwtAuthorize;

namespace JWTApiTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiJwtAuthorize(Configuration, (routeEndpoint, claims) =>
             {
                 return ValidatePermission(routeEndpoint, claims);
             });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting(); 
            app.UseAuthorization();
            //app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        public class Permission
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string Predicate { get; set; }
        }
        bool ValidatePermission(RouteEndpoint routeEndpoint, ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(m => m.Type.Equals(ClaimTypes.Name));
            if (claim == null)
            {
                return false;
            }
            if(claim.Value!= "gsw")
            {
                return false;
            }
            if(routeEndpoint.RoutePattern.RawText.ToLower()!="api/values")
            {
                return false;
            }
            return true;
        }
    }
}
