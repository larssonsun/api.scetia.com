using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static readonly bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            

            var section = Configuration.GetSection("IdentityServer4");
            var builder = services.AddIdentityServer()
                // .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources(section))
                .AddInMemoryClients(Config.GetClients(section))
                .AddInMemoryApiScopes(Config.GetApiScopes(section));

            if (isDevelopment)
            {
                // not recommended for production - you need to store your key material somewhere secure
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                var fileName = Path.Combine(Environment.CurrentDirectory, section["Certificates:CertPath"]);
                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException("Signing Certificate is missing!");
                }
                var cert = new X509Certificate2(fileName, section["Certificates:Password"]);
                builder.AddSigningCredential(cert);
            }

             services.AddTransient<IdentityServer4.Validation.ICustomTokenRequestValidator, DefaultClientClaimsAdder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }
    }
}
