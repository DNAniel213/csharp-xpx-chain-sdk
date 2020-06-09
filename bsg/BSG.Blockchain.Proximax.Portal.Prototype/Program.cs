using System;
using System.Threading.Tasks;
using System.Net;
using System.Json;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Proximax.Sirius.Sdk.NetCore.Examples
{
    class Program
    {
        private static readonly HttpClient httpclient = new HttpClient();


        static void Main(string[] args)
        {

            Program p = new Program();
            
            Account acc = new Account();
            acc.Id = 1;
            acc.PrivateKey = "testPrivateKey";
            acc.PublicKey = "lmaoAss";
            //p.TestAPOST(acc);
            var t = Task.Run(() => p.GetAsync());
            t.Wait();
            Console.WriteLine(t);

            /*
            var content = JsonConvert.SerializeObject(acc);

            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:5001/api/Accounts", UriKind.Absolute),
                Content = content,
            };
            var response = httpclient.SendAsync(request);
            Console.WriteLine(response.IsCompletedSuccessfully);
            */
        }


        public async Task<JsonValue> GetAsync()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(new Uri("https://localhost:5001/api/Accounts", UriKind.RelativeOrAbsolute));

            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonObject.Parse(content));
        }
        async void TestAPOST(Account acc)
        {
            var payload = JsonConvert.SerializeObject(acc);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:5001/api/Accounts", UriKind.RelativeOrAbsolute),
                Content = content,
            };
            var response = await httpclient.SendAsync(request);
            Console.WriteLine(response);
        }


        async void TestPOST(Account acc)
        {
            var client = new HttpClient();
            var payload = JsonConvert.SerializeObject(acc);

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(new Uri("https://localhost:5001/api/Accounts/", UriKind.RelativeOrAbsolute), content);
            Console.WriteLine(response);
            var x= 1;

        }

        



    }
    
public class Account
    {
        /// <summary>
        /// The unique identification that represents the user 
        /// </summary>
        /// <value></value>
        public long Id { get; set; }

        /// <summary>
        /// 
        /// The user's blockchain wallet address
        /// </summary>
        /// <value></value>
        public string WalletAddress { get; set; }

        /// <summary>
        /// The user's encrypted private key
        /// </summary>
        /// <value></value>
        public string PrivateKey { get; set; }

        /// <summary>
        /// The user's blockhain generated public key
        /// </summary>
        /// <value></value>
        public string PublicKey { get; set; }

        /// <summary>
        /// Just a test secret key for DTO
        /// </summary>
        /// <value></value>
        public string Secret { get; set;}
    }

}