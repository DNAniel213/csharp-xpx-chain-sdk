using XarcadeModels = Xarcade.Domain.Models;
using System;

namespace Xarcade.Api.Prototype
{
    public class Program
    {
        static void Main()
        {
            ProximaXProgram blockChain = new ProximaXProgram();
            RepositoryProgram repository = new RepositoryProgram();
            XarcadeModels.UserDTO user = new XarcadeModels.UserDTO();
            XarcadeModels.OwnerDTO owner = new XarcadeModels.OwnerDTO();
            XarcadeModels.OwnerDTO defaultOwner = new XarcadeModels.OwnerDTO
            {
                email    = "dnaniel213@gmail.com",
                userName = "dnaniel213",
                password = "encryptedpassword",
                fName    = "Daniel",
                lName    = "Fernandez"
            };

            Console.WriteLine("Enter 0 to Create Game");
            Console.WriteLine("Enter 1 to register");
            Console.WriteLine("Enter 2 to log in to existing account");
            Console.WriteLine("Enter 3 to use default account (skip auth)");
            Console.WriteLine("Enter 4 to just test the cryptography features");
            Console.WriteLine("input: ");
            string choice = Console.ReadLine();
            switch(choice)
            {
                case "0":  owner = repository.CreateGame(); break;
                case "1":  user = repository.Register(); break;
                case "2":  user = repository.Login();    break;
                case "3":  blockChain.ProximaXMain(defaultOwner); break;
            }

            Console.WriteLine("\nRedirecting to blockchain functions...");
            if(choice!="0")
                blockChain.ProximaXMain(user);
            else
                blockChain.ProximaXMain(owner);

            
        }
    }
}