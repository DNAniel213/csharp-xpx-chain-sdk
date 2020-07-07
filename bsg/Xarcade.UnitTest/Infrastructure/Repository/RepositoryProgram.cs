using System;
using System.Collections.Generic;
using Xarcade.Infrastructure.Repository;
using Xarcade.Domain.Authentication;

namespace Xarcade.Api.Prototype
{
    public class RepositoryProgram
    {
        DataAccessProximaX repo = new DataAccessProximaX();

        public XarcadeUserDTO Register()
        {
            var user = new XarcadeUserDTO();
            Console.WriteLine("\n==Register new User==");
            bool isRegistrationComplete = false, isEmailTaken = true, isUsernameTaken = true;

            while(!isRegistrationComplete)
            {
                while(isEmailTaken)
                {
                    Console.Write("Email: ");
                    user.Email = Console.ReadLine();

                    isEmailTaken = repo.portal.CheckExist("Authentication", repo.portal.CreateFilter(new KeyValuePair<string, string>("email", user.Email), FilterOperator.EQUAL));
                    if(isEmailTaken)
                        Console.WriteLine("Email already exists!");
                }

                while(isUsernameTaken)
                {
                    Console.Write("User name: ");
                    user.UserName = Console.ReadLine();

                    isUsernameTaken = repo.portal.CheckExist("Authentication", repo.portal.CreateFilter(new KeyValuePair<string, string>("userName", user.UserName), FilterOperator.EQUAL));
                    if(isUsernameTaken)
                        Console.WriteLine("Username already exists!");
                }

                Console.Write("Password: ");
                user.Password = Console.ReadLine();
                user.UserID = repo.portal.GetDocumentCount("Authentication");
                repo.SaveXarcadeUser(user);
                Console.WriteLine("Account Created!");
                return user;
            }

            return null;

        }

        public XarcadeUserDTO Login()
        {

            XarcadeUserDTO user = new XarcadeUserDTO();
            Console.WriteLine("\n==Log in==");
            bool isLoginComplete = false;

            while(!isLoginComplete)
            {

                Console.Write("Email or username: ");
                var login = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();

                var result = repo.portal.ReadDocument("Authentication", repo.portal.CreateFilter(new KeyValuePair<string, string>("email", login), FilterOperator.EQUAL));
                if(result == null)
                    result = repo.portal.ReadDocument("Authentication", repo.portal.CreateFilter(new KeyValuePair<string, string>("userName", login), FilterOperator.EQUAL));

                if(result != null)
                {
                    XarcadeUserDTO hit = BsonToModel.BsonToXarcadeUserDTO(result);

                    if(hit.Password == password)
                    {
                        isLoginComplete = true;
                        return hit;
                    }
                    else
                    {
                        Console.WriteLine("Credentials incorrect");
                    }
                    
                }


            }
            return null;
        }
    }
}