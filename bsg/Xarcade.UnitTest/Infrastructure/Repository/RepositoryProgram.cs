using System;
using System.Collections.Generic;
using XarcadeModels = Xarcade.Domain.Models;
using Xarcade.Api.Prototype.Repository;

namespace Xarcade.Api.Prototype
{
    public class RepositoryProgram
    {
        RepositoryPortal repo = new RepositoryPortal();

        public XarcadeModels.OwnerDTO CreateGame()
        {
            XarcadeModels.OwnerDTO owner = Register();
            Console.WriteLine("Game Created!");

            return owner;
        }
        public XarcadeModels.UserDTO Register()
        {
            XarcadeModels.UserDTO user = new XarcadeModels.UserDTO();
            Console.WriteLine("\n==Register new User==");
            bool isRegistrationComplete = false, isEmailTaken = true, isUsernameTaken = true;

            while(!isRegistrationComplete)
            {
                while(isEmailTaken)
                {
                    Console.Write("Email: ");
                    user.email = Console.ReadLine();

                    isEmailTaken = repo.CheckExist("Users", repo.CreateFilter(new KeyValuePair<string, string>("Email", user.email), FilterOperator.EQUAL));
                    if(isEmailTaken)
                        Console.WriteLine("Email already exists!");
                }

                while(isUsernameTaken)
                {
                    Console.Write("User name: ");
                    user.userName = Console.ReadLine();

                    isUsernameTaken = repo.CheckExist("Users", repo.CreateFilter(new KeyValuePair<string, string>("Username", user.userName), FilterOperator.EQUAL));
                    if(isUsernameTaken)
                        Console.WriteLine("Username already exists!");
                }

                Console.Write("Password: ");
                user.password = Console.ReadLine();
                Console.Write("First Name: ");
                user.fName = Console.ReadLine();
                Console.Write("Last Name: ");
                user.lName = Console.ReadLine();

                user.UserID = RepositoryPortal.GenerateID().GetHashCode();

                Console.WriteLine("Account Created!");
                repo.CreateDocument("Users", ModelToBson.AccountDTOtoBson(user));
                return user;
            }

            return null;

        }

        public XarcadeModels.UserDTO Login()
        {

            XarcadeModels.UserDTO user = new XarcadeModels.UserDTO();
            Console.WriteLine("\n==Log in==");
            bool isLoginComplete = false, areCredentialsCorrect = false;

            while(!isLoginComplete)
            {

                Console.Write("Email or username: ");
                var login = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();

                var result = repo.ReadDocument("Users", repo.CreateFilter(new KeyValuePair<string, string>("userName", login), FilterOperator.EQUAL));
                if(result == null)
                    result = repo.ReadDocument("Users", repo.CreateFilter(new KeyValuePair<string, string>("Email", login), FilterOperator.EQUAL));
                else if(result == null)
                    result = repo.ReadDocument("Owners", repo.CreateFilter(new KeyValuePair<string, string>("userName", login), FilterOperator.EQUAL));
                else if (result == null)
                    result = repo.ReadDocument("Owners", repo.CreateFilter(new KeyValuePair<string, string>("Email", login), FilterOperator.EQUAL));

                if(result != null)
                {
                    var hit = new XarcadeModels.UserDTO();
                    if(result.GetType().Equals(new XarcadeModels.UserDTO()))
                    {
                        hit = BsonToModel.BsonToUserDTO(result);
                    }
                    else
                    {
                        hit = BsonToModel.BsonToOwnerDTO(result);
                    }
                    
                    if(hit.password == password)
                    {
                        return hit;
                    }
                    
                }


            }
            return null;

        }
    }
}