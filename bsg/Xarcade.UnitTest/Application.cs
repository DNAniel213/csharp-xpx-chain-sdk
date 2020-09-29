using Xarcade.Domain.Authentication;
using Xarcade.Application.Xarcade;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.ProximaX;
using Xarcade.Infrastructure.Cryptography;

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

            CryptographyService cs = new CryptographyService();

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
            Console.WriteLine("Enter 6 to just test encryption");
            Console.WriteLine("Enter 7 to just test decryption");

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
                case "6":  var encrypted = cs.Encrypt("EC43AE1640952A963EF1BCEAB151B637121FE2CD1D0146533CF328E18E376DF7"); Console.WriteLine(encrypted); break;
                case "7":  var decrypted = cs.Decrypt("hREjyidDWJ3wPAyG/wfRjM+HTEAYs+BMcUBWRSspLEaBicHsXaDzI++gbtQv0dGyW1jxbPn0EmbjdBz5Do/hRlYxWvlIL/UiJsHQf8dPV4iPk8HaqELXqL1zUy5xADWCLm+Wg3FT2Z58r68R7jxRu3Aj7KsLSo+YNDXxGTzsx31MOmBnGj0sXCiMixSbdhEUjuM4LeA+wQW4CF0KxiOqFnangCJXdXU0ZAScdstFSZNF2nXZ/h0y39Cx9Ki9IXLSCpcuQm5Z2a/VcKXA/OlcxCt4cxrckkI5vpJ4zZB6QMS5WXK+pjEx5hyWxPIT7X8dgtNTJoOC5TbNounSIEre+A=="); Console.WriteLine(decrypted); break;
            }
*/
            blockChain.ProximaXMain(user, true);
        }
    }
}