using Invoice.Application;
using Invoice.Core;
using Invoice.WebAPI.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invoice.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //// Register the Swagger services
            services.AddSwaggerDocument(options =>
            {
                options.Title = "Invoice API";
                options.Version = "v1";
                options.Description = "";
                options.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
            });

            services.AddControllers();
            services.AddApplication();
            services.AddCore(Configuration, Environment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options =>
             options.WithOrigins("http://localhost:4200")
             .AllowAnyMethod()
             .AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi(t =>
                {
                    t.PostProcess = (document, request) =>
                    {
                        var bearerApiSecurityScheme = new OpenApiSecurityScheme
                        {
                            Type = OpenApiSecuritySchemeType.ApiKey,
                            Name = "Authorization",
                            Description = "Copy 'Bearer ' + valid JWT token into field",
                            In = OpenApiSecurityApiKeyLocation.Header
                        };

                        document.SecurityDefinitions.Add("JWT Token", bearerApiSecurityScheme);
                    };
                });

                app.UseSwaggerUi3(settings =>
                {
                    settings.Path = "/api";
                });
            }

            app.UseRouting();
            app.SeedDatabase(Configuration);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
