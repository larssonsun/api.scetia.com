using System;
using System.Threading;
using RestSharp;

namespace Timing
{
    class Program
    {
        static void Main(string[] args)
        {
            var Client = new RestClient("http://api.scetia.com");
            var request = new RestRequest("/gravel.app/customerUnit" + "", Method.GET);
            Console.WriteLine("started.");
            while (true)
            {
                var a = Client.Execute(request);
                Console.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + a.ResponseStatus);
                Thread.Sleep(18000);
            }
        }
    }
}
