using Invoice.Core.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invoice.WebAPI.Common.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void SeedDatabase(this IApplicationBuilder app, IConfiguration configuration)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                AppDbContextSeed.SeedAsync(services).GetAwaiter().GetResult();
            }
        }
    }
}
