using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using BSG.Blockchain.Portal.Proximax;
using BSG.Blockchain.Models;
using Newtonsoft.Json;
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
            var content = JsonConvert.SerializeObject(portal.GenerateWallet(1));

            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            var response = httpclient.PostAsync("https://localhost:5001/api/Accounts", byteContent);
            Console.WriteLine(response.IsCompletedSuccessfully);
            CreateHostBuilder(args).Build().Run();
        }
 



    }
    
}
