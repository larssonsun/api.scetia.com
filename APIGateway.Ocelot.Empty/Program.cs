using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace APIGateway.Ocelot.Empty
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(c =>
                {
                    c.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
                })

             .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseHttpSys(options =>
                    {
                        // The following options are set to default values.
                        options.Authentication.Schemes = AuthenticationSchemes.None;
                        options.Authentication.AllowAnonymous = true;
                        options.MaxConnections = null;
                        options.MaxRequestBodySize = 30000000;
                        options.UrlPrefixes.Add("http://api.scetia.com:80");
                    });
                });
    }
}
