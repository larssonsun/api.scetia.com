using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace APIGateway.Ocelot
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
                Console.Title = "api.scetia.com";
                Log.Information("Starting apigateway host");
                CreateHostBuilder(args).Build().Run();

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
            .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.AddJsonFile(isDevelopment ? "ocelot.Development.json" : "ocelot.json");
               })
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

                    webBuilder.UseStartup<Startup>().UseHttpSys();

                    // webBuilder.UseStartup<Startup>().UseHttpSys(options =>
                    // {
                    //     The following options are set to default values.
                    //     options.Authentication.Schemes = AuthenticationSchemes.None;
                    //     options.Authentication.AllowAnonymous = true;
                    //     options.MaxConnections = null;
                    //     options.MaxRequestBodySize = 30000000;
                    // });
                });
    }
}
