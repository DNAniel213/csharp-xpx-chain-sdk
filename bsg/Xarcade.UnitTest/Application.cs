using Xarcade.Domain.Authentication;
using System;

namespace Xarcade.Api.Prototype
{
    public class Program
    {
        static void Main()
        {
            ProximaXProgram blockChain = new ProximaXProgram();
            RepositoryProgram repository = new RepositoryProgram();
            XarcadeUserDTO user = new XarcadeUserDTO
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