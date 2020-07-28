using Xarcade.Domain.Authentication;
using Xarcade.Application.Xarcade;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.ProximaX;
using System;

namespace Xarcade.Api.Prototype
{
    public class Program
    {
        static void Main()
        {
            DataAccessProximaX access = new DataAccessProximaX();
            ProximaXProgram blockChain = new ProximaXProgram();
            ProximaxBlockchainPortal portal = new ProximaxBlockchainPortal();
            RepositoryProgram repository = new RepositoryProgram();
            TokenService ts = new TokenService(access,portal);
            TokenDto token = new TokenDto();
            GameDto game = new GameDto();
            XarcadeUser user = new XarcadeUser
            {
                UserID   = 0 ,
                Email    = "dnaniel213@gmail.com",
                UserName = "dnaniel213",
                Password = "encryptedpassword",
            };

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
                case "3":  var token1 = ts.CreateTokenAsync(token).GetAwaiter().GetResult();   break;
                case "5":  var game1 = ts.CreateGameAsync(game).GetAwaiter().GetResult();   break;
                case "6":  blockChain.ProximaXMain(user);   break;
                case "7":  blockChain.ProximaXMain(user);   break;
                case "8":  blockChain.ProximaXMain(user);   break;
                case "9":  blockChain.ProximaXMain(user);   break;
                case "0":  blockChain.ProximaXMain(user);   break;
            }

            blockChain.ProximaXMain(user, true);
                

            
        }
    }
}