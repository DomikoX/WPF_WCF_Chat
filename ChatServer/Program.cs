using System;
using System.ServiceModel;
using ChatLibrary;

namespace ChatServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(ServerService));
            host.Open();
            Console.WriteLine($"Service is ready .... Listenign on: net.tcp//localhost/Chat \nPress any key to terminate");
            Console.ReadKey();
        }
        
    }
}