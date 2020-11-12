using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace APIGateway.Ocelot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.AddJsonFile("ocelot.json");
               })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureKestrel(o =>
                    {

                        o.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(30);
                        o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
                    });
                });
    }
}
