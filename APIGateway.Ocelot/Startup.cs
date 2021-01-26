using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using System.Collections.Generic;

namespace APIGateway.Ocelot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            List<ApiResourceCfg> ApiResources = new List<ApiResourceCfg>();
            Configuration.Bind("IdentityServer4:ApiResources", ApiResources);

            var authenticationBuilder = services.AddAuthentication();

            // add authentication scheme from is4
            foreach (var apiResource in ApiResources)
            {
                authenticationBuilder.AddIdentityServerAuthentication(apiResource.AuthenticationScheme, jwtBearerOptions: options =>
                 {
                     options.RequireHttpsMetadata = apiResource.RequireHttpsMetadata;
                     options.Audience = apiResource.Audience;
                     options.Authority = Configuration["IdentityServer4:Authority"];
                 }, null);
            }

            // add others authentication scheme
            authenticationBuilder
            .AddJwtBearer("defaultJwtAuth", jbo =>
            {
                jbo.RequireHttpsMetadata = false;
                jbo.SaveToken = true;
                jbo.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["DefaultJwtAuth:Secret"])),
                    ValidIssuer = Configuration["DefaultJwtAuth:Issuer"],
                    ValidAudience = Configuration["DefaultJwtAuth:Audience"],

                };
            }).AddJwtBearer("Gravel.App", jbo =>
            {
                // should add claims to headers in ocelot.json settings otherwise will throw 402
                // 
                // "AddHeadersToRequest": {
                // "X-IS-MANAGER": "Claims[IsManager] > value > |",
                // "X-IS-SAMPLER": "Claims[IsSampler] > value > |",
                // "X-USER-NAME": "Claims[UserName] > value > |",
                // "X-USER-ID": "Claims[UserId] > value > |",
                // "X-SUPPLIERUNIT-ID": "Claims[SupplierUnitId] > value > |",
                // "X-USER-FULL-NAME": "Claims[UserFullName] > value > |"

                jbo.RequireHttpsMetadata = false;
                jbo.SaveToken = true;
                jbo.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["TokenGravelApp:Secret"])),
                    ValidIssuer = Configuration["TokenGravelApp:Issuer"],
                    ValidAudience = Configuration["TokenGravelApp:Audience"],

                };
            });

            services.AddAuthentication();

            services.AddOcelot()
            .AddConsul()
            .AddCacheManager(x => x.WithDictionaryHandle());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseOcelot().Wait();
        }
    }
}
