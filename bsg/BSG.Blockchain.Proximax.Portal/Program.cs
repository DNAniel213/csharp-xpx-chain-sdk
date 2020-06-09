using System;
using System.Net;
using System.Net.Http;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using BSG.Blockchain.Portal.Proximax;

namespace proximax_gamedev_rest
{
    public class Program
    {
        private static ProximaxBlockchainPortal portal = new ProximaxBlockchainPortal();
        private static readonly HttpClient httpclient = new HttpClient();
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                });

        public static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();
        }
 



    }
    
}
