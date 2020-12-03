using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace APIGateway.Ocelot
{
    public class Program
    {
        private static readonly bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.AddJsonFile(isDevelopment ? "ocelot.Development.json" : "ocelot.json");
               })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseHttpSys(options =>
                    {
                        // The following options are set to default values.
                        // options.Authentication.Schemes = AuthenticationSchemes.None;
                        // options.Authentication.AllowAnonymous = true;
                        // options.MaxConnections = null;
                        // options.MaxRequestBodySize = 30000000;
                    });
                });
    }
}
