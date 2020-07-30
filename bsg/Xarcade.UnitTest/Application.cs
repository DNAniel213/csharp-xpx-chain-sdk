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
                TokenId = 0,
                Name = null,
                Quantity = 0,
                Owner = user.UserID
            };

            Console.WriteLine("Enter 1 to Register");
            Console.WriteLine("Enter 2 to log in to existing account");
            Console.WriteLine("Enter 3 to use default account (skip auth)");
            Console.WriteLine("Enter 4 to just test the cryptography features");
            Console.Write("input: ");
            string choice = Console.ReadLine();
            switch(choice)
            {
                case "1":  user = repository.Register(); break;
                case "2":  user = repository.Login();    break; 
                case "3":  blockChain.ProximaXMain(user);   break;
            }

            blockChain.ProximaXMain(user, true);
                

            
        }
    }
}