using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace IdentityServer
{
    public class Program
    {
        private static readonly bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile(isDevelopment ? "appsettings.Development.json" : "appsettings.json")
            .Build();

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

            try
            {
                Console.Title = "IdentityServer.App";

                Log.Information("Starting web host");
                var host = CreateHostBuilder(args).Build();
                host.Run();
                
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Third-party log providers
                    webBuilder.UseSerilog((context, logger) =>
                    {
                        var configuration = new ConfigurationBuilder()
                        .AddJsonFile(isDevelopment ? "appsettings.Development.json" : "appsettings.json")
                        .Build();

                        logger.ReadFrom.Configuration(configuration);
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
