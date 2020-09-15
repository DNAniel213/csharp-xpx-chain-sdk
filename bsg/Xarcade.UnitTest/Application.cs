using Xarcade.Domain.Authentication;
using Xarcade.Application.Xarcade;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.ProximaX;

using Microsoft.Extensions.Configuration;

using System;

namespace Xarcade.Api.Prototype
{
    public class Program
    {
        static void Main()
        {
            ProximaXProgram blockChain = new ProximaXProgram();
            RepositoryProgram repository = new RepositoryProgram();

            DataAccessProximaX access = new DataAccessProximaX();
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json",false,true)
                .Build();
            ProximaxBlockchainPortal portal = new ProximaxBlockchainPortal(config);
            TokenService ts = new TokenService(access,portal);

            XarcadeUser user = new XarcadeUser
            {
                UserID   = ")",
            };
            XarcadeTokenDto xartokentest = new XarcadeTokenDto
            {
                TokenId = Guid.NewGuid().ToString(),
                Owner = user.UserID +""
            };
            TokenDto tokentest = new TokenDto
            {
                TokenId = Guid.NewGuid().ToString(),//TO ADD some randomness
                Name = "tokentest",
                Owner = user.UserID + ""
            };
            //Console.WriteLine("Duration of the game (days):");
            //ulong duration = Convert.ToUInt32(Console.ReadLine());
            GameDto gametest = new GameDto
            {
                GameId = Guid.NewGuid().ToString(),//should be generated
                Name = "token",
                Owner = user.UserID + "",
                //Expiry = DateTime.Now.AddDays(364)
            };
            //long tokID = 637321661762507047;
            //Console.Write("Number of Days:  ");
            ulong days = Convert.ToUInt32(100);//take note of the remaining duration of the namespace | 365 days max
            ulong duration = days * 86400/15;

            Console.WriteLine("Enter 1 to Register");
            Console.WriteLine("Enter 2 to log in to existing account");
            Console.WriteLine("Enter 3 to use default account (skip auth)");
            Console.WriteLine("Enter 4 to just test the cryptography features");
            Console.WriteLine("Enter 5 to just test the get game info feature");

            Console.Write("input: ");
            string choice = Console.ReadLine();
            /*
            switch(choice)
            {
                case "1":  user = repository.Register(); break;
                case "2":  user = repository.Login();    break; 
                case "3":  blockChain.ProximaXMain(user);   break;
                case "5":  var gameinfo = ts.GetGameInfoAsync(1 + "").GetAwaiter().GetResult(); Console.WriteLine(gameinfo); break;
                //case "0":  var result = ts.CreateTokenAsync(tokentest).GetAwaiter().GetResult(); break;
            }
*/
            blockChain.ProximaXMain(user, true);
        }
    }
}