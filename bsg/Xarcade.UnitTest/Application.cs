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
                UserID   = 0 ,
                Email    = "dnaniel213@gmail.com",
                UserName = "dnaniel213",
                Password = "encryptedpassword",
            };

            TokenDto tokentest = new TokenDto
            {
                TokenId = Convert.ToUInt64(DateTime.Now.Ticks),//should be generated
                Name = "tokentest",
                Quantity = 0,
                Owner = user.UserID
            };
            //Console.WriteLine("Duration of the game (days):");
            //ulong duration = Convert.ToUInt32(Console.ReadLine());
            GameDto creategametest = new GameDto
            {
                GameID = DateTime.Now.Ticks,//should be generated
                Name = "gametest2",
                Owner = user.UserID,
                //Expiry = DateTime.Now.AddDays(364)
            };
            long tokID = 637321661762507047;
            //Console.Write("Number of Days:  ");
            ulong days = Convert.ToUInt32(100);//take note of the remaining duration of the namespace | 365 days max
            ulong duration = days * 86400/15;

            Console.WriteLine("Enter 1 to Register");
            Console.WriteLine("Enter 2 to log in to existing account");
            Console.WriteLine("Enter 3 to use default account (skip auth)");
            Console.WriteLine("Enter 4 to just test the cryptography features");
            Console.WriteLine("Enter 5 to just test the create token feature");
            Console.WriteLine("Enter 6 to just test the create game feature");
            Console.WriteLine("Enter 7 to just test the extend game feature");
            Console.WriteLine("Enter 8 to just test the modify token supply feature");
            Console.WriteLine("Enter 9 to just test the get token info feature");
            Console.WriteLine("Enter 0 to just test the get game info feature");

            Console.Write("input: ");
            string choice = Console.ReadLine();
            switch(choice)
            {
                case "1":  user = repository.Register(); break;
                case "2":  user = repository.Login();    break; 
                case "3":  blockChain.ProximaXMain(user);   break;
                case "5":  var ct = ts.CreateTokenAsync(tokentest,"naisu2").GetAwaiter().GetResult(); break;
                case "6":  var cg= ts.CreateGameAsync(creategametest).GetAwaiter().GetResult(); break;
                case "7":  var extendgame = ts.ExtendGameAsync(creategametest,duration).GetAwaiter().GetResult(); break;
                case "8":  var modsupply= ts.ModifyTokenSupplyAsync(tokentest).GetAwaiter().GetResult(); break;
                case "9":  var gt = ts.GetTokenInfoAsync(tokID).GetAwaiter().GetResult(); break;
                //case "0":  var result = ts.CreateTokenAsync(tokentest).GetAwaiter().GetResult(); break;
            }

            blockChain.ProximaXMain(user, true);
        }
    }
}