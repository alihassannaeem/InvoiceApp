using Invoice.Application.Auth.Identity;
using Invoice.Application.Auth.Jwt;
using Invoice.Application.Common.Interfaces;
using Invoice.Core.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Invoice.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration Configuration, IWebHostEnvironment environment)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions =>
                    {
                        sqlServerOptions.CommandTimeout(120);
                        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 3);
                        sqlServerOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    }).UseLazyLoadingProxies(false));
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<AppDbContext>());
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();            
            services.AddScoped<IJwtFactory, JwtFactory>();

            #region Auth 

            var builder = services.AddIdentityCore<AppUser>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 8;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            builder.AddRoleManager<RoleManager<IdentityRole>>();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(options =>
            {
                RSA rsa = RSA.Create();
                rsa.ImportRSAPrivateKey(
                    source: Convert.FromBase64String(jwtAppSettingOptions["PrivateKey"]),
                    bytesRead: out int _
                );

                var signingCredentials = new SigningCredentials(
                    key: new RsaSecurityKey(rsa),
                    algorithm: SecurityAlgorithms.RsaSha256 // Important to use RSA version of the SHA algo 
                );

                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = signingCredentials;
                options.ValidFor = TimeSpan.FromDays(1);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                RSA rsa = RSA.Create();
                rsa.ImportRSAPublicKey(
                    source: Convert.FromBase64String(jwtAppSettingOptions["PublicKey"]),
                    bytesRead: out int _
                );

                var externalSecurityKeys = new List<SecurityKey>();
                foreach (var trustedPublicKeys in jwtAppSettingOptions.GetSection("TrustedPublicKeys").Get<string[]>())
                {
                    var rsaExternal = RSA.Create();
                    rsaExternal.ImportRSAPublicKey(
                        source: Convert.FromBase64String(trustedPublicKeys),
                        bytesRead: out int _
                    );
                    externalSecurityKeys.Add(new RsaSecurityKey(rsaExternal));
                }

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                    ValidIssuers = new List<string>() { "InvoiceAPI" },

                    ValidateAudience = true,
                    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                    ValidAudiences = new List<string>() { "http://localhost:60993/" },

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    IssuerSigningKeys = externalSecurityKeys,

                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };


                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            #endregion

            return services;

        }
    }
}
