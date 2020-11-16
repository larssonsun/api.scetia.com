using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
class Program
{
    static async Task<int> Main(string[] args)
    {
        // var Client = new RestClient("http://api.scetia.com");
        // var request = new RestRequest("/obsolete.df/user" + "", Method.GET);
        // Console.WriteLine("started.");
        // while (true)
        // {
        //     var a = Client.Execute(request);
        //     // Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss")} - {a.StatusCode} {a.StatusDescription}");
        //     Thread.Sleep(15000);
        // }

        var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient();
                services.AddTransient<IMyService, MyService>();
            }).UseConsoleLifetime();

        var host = builder.Build();

        using (var serviceScope = host.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;

            try
            {
                var myService = services.GetRequiredService<IMyService>();

                var pageContent = await myService.GetPage();

                // Console.WriteLine(pageContent.Substring(0, 5000));
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();

                logger.LogError(ex, "An error occurred.");
            }
        }

        return 0;
    }

    public interface IMyService
    {
        Task<string> GetPage();
    }

    public class MyService : IMyService
    {
        private readonly IHttpClientFactory _clientFactory;

        public MyService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetPage()
        {
            // Content from BBC One: Dr. Who website (©BBC)
            var request = new HttpRequestMessage(HttpMethod.Get,
                "http://localhost/web/shity.html");

            var client = _clientFactory.CreateClient();

            var s = DateTime.Now;

            var response = await client.SendAsync(request);

            Console.WriteLine();
            Console.WriteLine($"StatusCode: {response.StatusCode} " + (DateTime.Now - s).TotalMilliseconds + "ms");
            Console.ReadLine();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"StatusCode: {response.StatusCode}";
            }
        }
    }
}