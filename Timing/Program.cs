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
            var request = new RestRequest("/obsolete.df/user", Method.GET);
            Console.WriteLine("started.");
            while(true)
            {
                Client.Execute(request);
                Thread.Sleep(20000);
            }
        }
    }
}
