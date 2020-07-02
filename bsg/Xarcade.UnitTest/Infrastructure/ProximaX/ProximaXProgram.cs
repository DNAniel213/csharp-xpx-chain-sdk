using System.Collections.Generic;
using System;
using XarcadeModels = Xarcade.Domain.Models;
using XarcadeParams = Xarcade.Domain.Params;
using Xarcade.Api.Prototype.Blockchain;
using Xarcade.Api.Prototype.Repository;
/*
    BSG ACCOUNT
    wallet uid: blackspeargames
    password: bsg696969
*/

namespace Xarcade.Api.Prototype
{
    class ProximaXProgram
    {
        RepositoryPortal repo = new RepositoryPortal();

        public ProximaxBlockchainPortal portal = null;
        public ProximaxAccount pAccount = null;
        public ProximaxMosaic pMosaic = null;
        public ProximaxNamespace pNamespace = null;
        public ProximaxTransaction pTransaction = null;

        public const string TEST_PRIVATE_BSG_1 = "1A90826869ECCBEF591B049745BF3C17EC2A7FA1E9C91787C711194165FE2034"; //BSG 1's account
        public const string TEST_PRIVATE_BSG_2= "8C0C98ED0D0D703B56EB5FCBA55B02BB9661153C44D2C782F54846D902CEC4B5"; //BSG 2's account
        public const string namespaceName = "hola";
        public const string subNamespaceName = "bigfoo";
        public void ProximaXMain(XarcadeModels.AccountDTO user, bool isNewAccount = false)
        {
            portal = new ProximaxBlockchainPortal();
            pAccount = new ProximaxAccount(portal);
            pMosaic = new ProximaxMosaic(portal);
            pNamespace = new ProximaxNamespace(portal);
            pTransaction = new ProximaxTransaction(portal);
            XarcadeModels.AccountDTO testAccount = pAccount.CreateAccount(0, TEST_PRIVATE_BSG_1);
            XarcadeModels.MosaicDTO  testMosaic  = null;
            XarcadeModels.NamespaceDTO  testNamespace  = null;

            if(isNewAccount)
            {
                testAccount = pAccount.CreateAccount(0);
                user.WalletAddress    = testAccount.WalletAddress;
                user.PrivateKey       = testAccount.PrivateKey;
                user.PublicKey        = testAccount.PublicKey;
                user.Created          = DateTime.Now;
            }
            else
            {
                
            }
        
            for (int i = 0; true; i++)
            {
                Console.WriteLine("\n==Xarcade Progress Test Program==");
                //Console.WriteLine("Enter '0' to create a new account");
                Console.WriteLine("Enter '1' to send xpx to the new account");
                Console.WriteLine("Enter '2' to list transactions of the new account");
                Console.WriteLine("Enter '3' to create a new mosaic");
                Console.WriteLine("Enter '4' Modify mosaic supply");
                Console.WriteLine("Enter '5' Send mosaic to BSG 1 Wallet");
                Console.WriteLine("Enter '6' Create namespace");
                Console.WriteLine("Enter '7' Create subnamespace");
                Console.WriteLine("Enter '8' Link Namespace to mosaic");
                
                Console.Write("input: ");
                string choice = Console.ReadLine();
                switch(choice)
                {
                    case "0" : testAccount = CreateAccountTest(); break;
                    case "1" : SendXPXTest(testAccount); break;
                    case "2" : GetAccountTransactions(testAccount); break;
                    case "3" : testMosaic = CreateMosaicTest(testAccount); break;
                    case "4" : ModifyMosaicSupplyTest(testAccount, testMosaic); break;
                    case "5" : SendMosaicTest(testAccount, testMosaic); break;
                    case "6" : testNamespace = CreateNamespaceTest(testAccount); break;   //TODO make this use the test account to sign
                    case "7" : CreateSubNamespaceTest(testAccount); break;           //TODO make this use the test account to sign
                    case "8" : LinkMosaicToNamespaceTest(testAccount, testMosaic, testNamespace); break;   //TODO Link it with created mosaic
                }
                Console.WriteLine("\nPress any key to proceed..");
                System.Console.ReadKey();
            }

        }

        //read the function name
        private XarcadeModels.AccountDTO CreateAccountTest()
        {
            Console.Write("Enter User ID: ");
            XarcadeModels.AccountDTO newAccount = pAccount.CreateAccount(Convert.ToUInt32(Console.ReadLine()));
            Console.WriteLine(newAccount.ToString());
            return newAccount;
        }

        private void SendXPXTest(XarcadeModels.AccountDTO newAccount)
        {   
            Console.WriteLine("\nHow much XPX should we send to account " + newAccount.UserID + " ?:");
            XarcadeModels.AccountDTO dane2 = pAccount.CreateAccount(2, TEST_PRIVATE_BSG_1);
            XarcadeParams.SendXpxParams param = new XarcadeParams.SendXpxParams();
            param.sender = dane2;
            param.recepientAddress = newAccount.WalletAddress;
            param.amount =  Convert.ToUInt64(Console.ReadLine());
            param.message = "Pls work.jpg";

            Console.WriteLine("\nSending " + param.amount + " xpx to account " + newAccount.UserID + "...");
            XarcadeModels.TransactionDTO sendCurrency = pTransaction.SendXPXAsync(param).GetAwaiter().GetResult();
            Console.WriteLine(sendCurrency.ToString());
            Console.WriteLine(sendCurrency.Asset.ToString());
            Console.WriteLine("Transaction signed and announced!");
            
            var tracking = pTransaction.MonitorTransactionAsync(sendCurrency,param).GetAwaiter().GetResult();
            // TODO Add tracking here
        }

        private void GetAccountTransactions(XarcadeModels.AccountDTO newAccount)
        {
            List<XarcadeModels.TransactionDTO> transactions = pAccount.GetAccountTransactions(newAccount.WalletAddress, 10).GetAwaiter().GetResult();
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
        private XarcadeModels.MosaicDTO CreateMosaicTest(XarcadeModels.AccountDTO newAccount)
        {
            XarcadeParams.CreateMosaicParams param = new XarcadeParams.CreateMosaicParams();
            param.accountDTO = pAccount.CreateAccount(1, newAccount.PrivateKey); 
            XarcadeModels.MosaicDTO createMosaicT = pMosaic.CreateMosaicAsync(param).GetAwaiter().GetResult(); 
            
            Console.WriteLine(createMosaicT.ToString());
            return createMosaicT;
            // TODO Add tracking here
        }

        private void ModifyMosaicSupplyTest(XarcadeModels.AccountDTO newAccount, XarcadeModels.MosaicDTO newMosaic)
        {
            Console.Write("Amount to modify:  ");
            int am = Convert.ToInt32(Console.ReadLine());
            XarcadeParams.ModifyMosaicSupplyParams param = new XarcadeParams.ModifyMosaicSupplyParams
            {
                accountDTO = newAccount,
                mosaicID = newMosaic.MosaicID,
                amount = am
            };
            Console.Write("Modifying " + newMosaic.MosaicID + " supply by " + am );
            XarcadeModels.MosaicDTO modifyMosaicT = pMosaic.ModifyMosaicSupply(param).GetAwaiter().GetResult();
            // TODO Add tracking here

        }

        //Sends mosaic from Dane1 to Dane2 accounts. Usually, only need PUBLIC KEY from recepient account in real application
        private void SendMosaicTest(XarcadeModels.AccountDTO newAccount, XarcadeModels.MosaicDTO newMosaic)
        {
            Console.Write("Amount to send:  ");
            string amountToSend = Console.ReadLine();
            Console.Write("Message:  ");
            string message = Console.ReadLine();
            XarcadeParams.SendMosaicParams param = new XarcadeParams.SendMosaicParams
            {
                mosaicID = pMosaic.GetMosaicAsync(newMosaic.MosaicID).GetAwaiter().GetResult().MosaicID,
                sender = pAccount.CreateAccount(1, newAccount.PrivateKey),
                recepientAddress = pAccount.CreateAccount(2, TEST_PRIVATE_BSG_1).WalletAddress,
                amount = Convert.ToUInt64(amountToSend),
                message = message
            };

            XarcadeModels.TransactionDTO sendCoinT = pMosaic.SendMosaicAsync(param).GetAwaiter().GetResult();
        }
    
        //Creates a namespace using Bruh's Private Key
    
        private XarcadeModels.NamespaceDTO CreateNamespaceTest(XarcadeModels.AccountDTO newAccount)
        {
            Console.Write("Namespace name:  ");
            string name = Console.ReadLine();
            XarcadeParams.CreateNamespaceParams param = new XarcadeParams.CreateNamespaceParams();
            param.accountDTO = pAccount.CreateAccount(1, newAccount.PrivateKey);
            param.Domain = name;
            XarcadeModels.NamespaceDTO createNamespaceT = pNamespace.CreateNamespaceAsync(param).GetAwaiter().GetResult();

            Console.WriteLine(createNamespaceT.ToString());
            return createNamespaceT;
        }

        //Creates a subnamespace using Bruh's private key and Parent namespace
        private XarcadeModels.NamespaceDTO CreateSubNamespaceTest(XarcadeModels.AccountDTO newAccount)
        {
            Console.Write("Parent Namespace name:  ");
            string parentName = Console.ReadLine();
            Console.Write("Child Namespace name:  ");
            string childName = Console.ReadLine();
            XarcadeParams.CreateNamespaceParams param = new XarcadeParams.CreateNamespaceParams();
            param.accountDTO = pAccount.CreateAccount(1, newAccount.PrivateKey);
            param.Domain = parentName;
            param.LayerOne = childName;

            XarcadeModels.NamespaceDTO createNamespaceT = pNamespace.CreateSubNamespaceAsync(param).GetAwaiter().GetResult();
            
            Console.WriteLine(createNamespaceT.ToString());
            return createNamespaceT;
        }
        
        private void LinkMosaicToNamespaceTest(XarcadeModels.AccountDTO newAccount, XarcadeModels.MosaicDTO newMosaic, XarcadeModels.NamespaceDTO newNamespace)
        {
            Console.Write("Namespace to link:  ");
            string name = Console.ReadLine();
            XarcadeParams.LinkMosaicParams param = new XarcadeParams.LinkMosaicParams
            {
                accountDTO   =  pAccount.CreateAccount(1, newAccount.PrivateKey),
                mosaicID     = pMosaic.GetMosaicAsync(newMosaic.MosaicID).GetAwaiter().GetResult().MosaicID,
                namespaceDTO = pNamespace.GetNamespaceInformation(name).GetAwaiter().GetResult()
            };

            var link = pMosaic.LinkMosaicAsync(param).GetAwaiter().GetResult();
        }


    }
}
