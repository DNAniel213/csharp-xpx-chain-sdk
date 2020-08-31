using System.Collections.Generic;
using System;
using Xarcade.Domain.ProximaX;
using Xarcade.Infrastructure.ProximaX.Params;
using Xarcade.Infrastructure.ProximaX;
using Xarcade.Domain.Authentication;
using Xarcade.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
/*
    BSG ACCOUNT
    wallet uid: blackspeargames
    password: bsg696969
*/

namespace Xarcade.Api.Prototype
{
    class ProximaXProgram
    {
        DataAccessProximaX repo = new DataAccessProximaX();

        public ProximaxBlockchainPortal portal = null;
        public const string TEST_PRIVATE_BSG_1 = "1A90826869ECCBEF591B049745BF3C17EC2A7FA1E9C91787C711194165FE2034"; //BSG 1's account
        public const string TEST_PRIVATE_BSG_2= "8C0C98ED0D0D703B56EB5FCBA55B02BB9661153C44D2C782F54846D902CEC4B5"; //BSG 2's account
        public const string namespaceName = "hola";
        public const string subNamespaceName = "bigfoo";
        public void ProximaXMain(XarcadeUser user, bool isNewAccount = false)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            portal = new ProximaxBlockchainPortal(config);
            Account testAccount = portal.CreateAccountAsync(999 +"", TEST_PRIVATE_BSG_1).GetAwaiter().GetResult();
            Mosaic  testMosaic  = null;
            Namespace  testNamespace  = null;

            Console.WriteLine(user.ToString());
        
            for (int i = 0; true; i++)
            {
                Console.WriteLine("\n==Xarcade Progress Test Program==");
                Console.WriteLine("Enter '0' to create a new wallet");
                Console.WriteLine("Enter '1' to send xpx to the new account");
                Console.WriteLine("Enter '2' to list transactions of the new account");
                Console.WriteLine("Enter '3' to create a new mosaic");
                Console.WriteLine("Enter '4' Modify mosaic supply");
                Console.WriteLine("Enter '5' Send mosaic to BSG 1 Wallet");
                Console.WriteLine("Enter '6' Create namespace");
                Console.WriteLine("Enter '7' Create subnamespace");
                Console.WriteLine("Enter '8' Link Namespace to mosaic");
                Console.WriteLine("Enter '9' to create a wallet for other user");
                Console.WriteLine("Enter 'a' to list user wallets");
                Console.WriteLine("Enter 'b' to get user token balance");

                
                Console.Write("input: ");
                string choice = Console.ReadLine();
                switch(choice)
                {
                    case "0" : testAccount = CreateOwnAccount(user.UserID +""); break;
                    case "1" : SendXPXTest(testAccount); break;
                    case "2" : GetAccountTransactions(testAccount); break;
                    case "3" : testMosaic = CreateMosaicTest(testAccount); break;
                    case "4" : ModifyMosaicSupplyTest(testAccount, testMosaic); break;
                    case "5" : SendMosaicTest(testAccount, testMosaic); break;
                    case "6" : testNamespace = CreateNamespaceTest(testAccount); break;   //TODO make this use the test account to sign
                    case "7" : CreateSubNamespaceTest(testAccount); break;           //TODO make this use the test account to sign
                    case "8" : LinkMosaicToNamespaceTest(testAccount, testMosaic, testNamespace); break;   //TODO Link it with created mosaic
                    case "9" : CreateAccountForUser(user.UserID + ""); break;   //TODO Link it with created mosaic
                    case "a" : ListUserWallets(user); break;   //TODO Link it with created mosaic
                    case "b" : GetTokenBalance(testAccount); break;   
                }
                Console.WriteLine("\nPress any key to proceed..");
                System.Console.ReadKey();
            }

        }

        private void GetTokenBalance(Account account)
        {
            Console.Write("Token name:  ");
            string tokenName = Console.ReadLine(); 
            Asset tokenbalance = portal.GetTokenBalanceAsync(account,tokenName).GetAwaiter().GetResult();
            Console.WriteLine(tokenbalance.ToString());
        }

        private void ListUserWallets(XarcadeUser xarUserDTO)
        {
            var result = repo.portal.ReadCollection("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("userID", xarUserDTO.UserID), FilterOperator.EQUAL));
        }

        //read the function name
        private Owner CreateOwnAccount(string userID)
        {
            Account newWallet = portal.CreateAccountAsync(userID).GetAwaiter().GetResult();

            var ownerDTO = new Owner
            {
                UserID        = newWallet.UserID,
                WalletAddress = newWallet.WalletAddress,
                PrivateKey    = newWallet.PrivateKey,
                PublicKey     = newWallet.PublicKey,
                Created       = newWallet.Created
            };
            repo.SaveOwner(ownerDTO);
            Console.WriteLine(ownerDTO.ToString());

            return ownerDTO;
        }

        private User CreateAccountForUser(string ownerID)
        {
            Console.Write("Enter User ID: ");
            Account newWallet = portal.CreateAccountAsync(Console.ReadLine()).GetAwaiter().GetResult();
            var userDTO = new User
            {
                UserID        = newWallet.UserID,
                WalletAddress = newWallet.WalletAddress,
                PrivateKey    = newWallet.PrivateKey,
                PublicKey     = newWallet.PublicKey,
                Created       = newWallet.Created,
                OwnerID       = ownerID
            };
            Console.WriteLine(userDTO.ToString());
            repo.SaveUser(userDTO);
            return userDTO;
        }

        private void SendXPXTest(Account newAccount)
        {   
            Console.Write("\nHow much XPX should we send to account " + newAccount.UserID + " ?:");
            Account dane2 = portal.CreateAccountAsync(999 +"", TEST_PRIVATE_BSG_1).GetAwaiter().GetResult();
            var param = new SendXpxParams();
            param.Sender = dane2;
            param.RecepientAddress = newAccount.WalletAddress;
            param.Amount =  Convert.ToUInt64(Console.ReadLine());
            param.Message = "Pls work.jpg";
            Console.WriteLine(dane2.ToString());
            Console.WriteLine("\nSending " + param.Amount + " xpx to account " + newAccount.UserID + "...");
            Transaction sendCurrency = portal.SendXPXAsync(param).GetAwaiter().GetResult();
            Console.WriteLine(sendCurrency.ToString());
            Console.WriteLine(sendCurrency.Asset.ToString());
            Console.WriteLine("Transaction signed and announced!");
            
            repo.SaveTransaction(sendCurrency);

            // TODO Add tracking here
        }

        private void GetAccountTransactions(Account newAccount)
        {
            List<Transaction> transactions = portal.GetAccountTransactionsAsync(newAccount.WalletAddress, 10).GetAwaiter().GetResult();
            int i = 0;
            if(transactions.Count > 0)
            {
                foreach (var tx in transactions)
                {
                    Console.WriteLine("Transaction #" +i);
                    Console.WriteLine(tx.ToString());
                    i++;
                }
            }
            else
            {
                Console.WriteLine("\nNo Transactions Found..");
            }



        }


        //Creates a mosaic using Dane's private key
        private Mosaic CreateMosaicTest(Account newAccount)
        {
            var param = new CreateMosaicParams();
            param.Account = portal.CreateAccountAsync(1 +"", newAccount.PrivateKey).GetAwaiter().GetResult(); 
            Mosaic createMosaicT = portal.CreateMosaicAsync(param).GetAwaiter().GetResult().tMosaic; 
            
            Console.WriteLine(createMosaicT.ToString());
            repo.SaveMosaic(createMosaicT);

            return createMosaicT;
            // TODO Add tracking here
        }

        private void ModifyMosaicSupplyTest(Account newAccount, Mosaic newMosaic)
        {
            Console.Write("Amount to modify:  ");
            int am = Convert.ToInt32(Console.ReadLine());
            var param = new ModifyMosaicSupplyParams
            {
                Account = newAccount,
                MosaicID = newMosaic.MosaicID + "",
                Amount = am
            };
            Console.Write("Modifying " + newMosaic.MosaicID + " supply by " + am );
            Transaction modifyMosaicT = portal.ModifyMosaicSupplyAsync(param).GetAwaiter().GetResult();
            
            repo.SaveTransaction(modifyMosaicT);

            // TODO Add tracking here

        }

        //Sends mosaic from Dane1 to Dane2 accounts. Usually, only need PUBLIC KEY from recepient account in real application
        private void SendMosaicTest(Account newAccount, Mosaic newMosaic)
        {
            Console.Write("Amount to send:  ");
            string amountToSend = Console.ReadLine();
            Console.Write("Message:  ");
            string message = Console.ReadLine();
            var param = new SendMosaicParams
            {
                MosaicID = portal.GetMosaicAsync(newMosaic.MosaicID +"").GetAwaiter().GetResult().MosaicID,
                Sender = portal.CreateAccountAsync(1 + "", newAccount.PrivateKey).GetAwaiter().GetResult(),
                RecepientAddress = portal.CreateAccountAsync(2 + "", TEST_PRIVATE_BSG_1).GetAwaiter().GetResult().WalletAddress,
                Amount = Convert.ToUInt64(amountToSend),
                Message = message
            };

            Transaction sendCoinT = portal.SendMosaicAsync(param).GetAwaiter().GetResult();
            repo.SaveTransaction(sendCoinT);

        }
    
        //Creates a namespace using Bruh's Private Key
    
        private Namespace CreateNamespaceTest(Account newAccount)
        {
            Console.Write("Namespace name:  ");
            string name = Console.ReadLine();
            var param = new CreateNamespaceParams();
            param.Account = portal.CreateAccountAsync(1 + "", newAccount.PrivateKey).GetAwaiter().GetResult();
            param.Domain = name;
            var createNamespaceT = portal.CreateNamespaceAsync(param).GetAwaiter().GetResult();

            Console.WriteLine(createNamespaceT.ToString());
            repo.SaveNamespace(createNamespaceT.gameName);
            
            return createNamespaceT.gameName;
        }

        //Creates a subnamespace using Bruh's private key and Parent namespace
        private Namespace CreateSubNamespaceTest(Account newAccount)
        {
            Console.Write("Parent Namespace name:  ");
            string parentName = Console.ReadLine();
            Console.Write("Child Namespace name:  ");
            string childName = Console.ReadLine();
            var param = new CreateNamespaceParams();
            param.Account = portal.CreateAccountAsync(1 +"", newAccount.PrivateKey).GetAwaiter().GetResult();
            param.Parent = parentName;
            param.Domain = childName;

            var createNamespaceT = portal.CreateNamespaceAsync(param).GetAwaiter().GetResult();
            repo.SaveNamespace(createNamespaceT.gameName);
            
            Console.WriteLine(createNamespaceT.ToString());
            return createNamespaceT.gameName;
        }
        
        private void LinkMosaicToNamespaceTest(Account newAccount, Mosaic newMosaic, Namespace newNamespace)
        {
            Console.Write("Namespace to link:  ");
            string name = Console.ReadLine();
            var param = new LinkMosaicParams
            {
                Account   =  portal.CreateAccountAsync(1 + "", newAccount.PrivateKey).GetAwaiter().GetResult(),
                MosaicID     = portal.GetMosaicAsync(newMosaic.MosaicID).GetAwaiter().GetResult().MosaicID,
                Namespace = portal.GetNamespaceInformationAsync(name).GetAwaiter().GetResult()
            };

            var link = portal.LinkMosaicAsync(param).GetAwaiter().GetResult();
        }

        private Namespace ExtendNamespaceDurationTest(Account newAccount)
        {
            Console.Write("Namespace name:  ");
            string namespaceName = Console.ReadLine();
            Console.Write("Number of Days:  ");
            ulong days = Convert.ToUInt64(Console.ReadLine());//take note of the remaining duration of the namespace | 365 days max
            ulong duration = days * 86400/15;
            
            var param = new CreateNamespaceParams();
            param.Account = portal.CreateAccountAsync(1 +"", newAccount.PrivateKey).GetAwaiter().GetResult();
            param.Domain = namespaceName;
            

            var account = portal.CreateAccountAsync(1 +"", newAccount.PrivateKey);
            var namespaceInfo = portal.GetNamespaceInformationAsync(namespaceName).GetAwaiter().GetResult();
            Namespace extendNamespace = portal.ExtendNamespaceDurationAsync(namespaceName,newAccount.PrivateKey,namespaceInfo,param).GetAwaiter().GetResult();
            repo.SaveNamespace(extendNamespace);

            Console.WriteLine(extendNamespace.ToString());
            return extendNamespace;
        }
    }
}
