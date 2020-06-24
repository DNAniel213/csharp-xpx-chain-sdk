using System.Collections.Generic;
using System;
using XarcadeModels = Xarcade.Domain.Models;
using XarcadeParams = Xarcade.Domain.Params;
using Xarcade.Api.Prototype.Blockchain;
using MongoDB.Driver;
/*
    BSG ACCOUNT
    wallet uid: blackspeargames
    password: bsg696969
*/

namespace Xarcade.Api.Prototype
{
    class Program
    {
        public ProximaxBlockchainPortal portal = null;
        public ProximaxAccount pAccount = null;
        public ProximaxMosaic pMosaic = null;
        public ProximaxNamespace pNamespace = null;
        public ProximaxTransaction pTransaction = null;

        public const string TEST_PRIVATE_BSG_1 = "1A90826869ECCBEF591B049745BF3C17EC2A7FA1E9C91787C711194165FE2034"; //BSG 1's account
        public const string TEST_PRIVATE_BSG_2= "8C0C98ED0D0D703B56EB5FCBA55B02BB9661153C44D2C782F54846D902CEC4B5"; //BSG 2's account
        public const string namespaceName = "foobar";
        public const string subNamespaceName = "bigfoo";
        static void Main(string[] args)
        {
            Program p = new Program();
            p.portal = new ProximaxBlockchainPortal();
            p.pAccount = new ProximaxAccount(p.portal);
            p.pMosaic = new ProximaxMosaic(p.portal);
            p.pNamespace = new ProximaxNamespace(p.portal);
            p.pTransaction = new ProximaxTransaction(p.portal);
            XarcadeModels.AccountDTO testAccount = p.pAccount.CreateAccount(0, TEST_PRIVATE_BSG_1);
            XarcadeModels.MosaicDTO  testMosaic  = null;
            XarcadeModels.NamespaceDTO  testNamespace  = null;

            
            //var client = new MongoClient("mongodb+srv://dane:pikentz213@xarcadeprotodb-ctyys.mongodb.net/<dbname>?retryWrites=true&w=majority");
            //var database = client.GetDatabase("test");

            for (int i = 0; true; i++)
            {
                Console.WriteLine("\n==Xarcade Progress Test Program==");
                Console.WriteLine("Enter '0' to create a new account");
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
                    case "0" : testAccount = p.CreateAccountTest(); break;
                    case "1" : p.SendXPXTest(testAccount); break;
                    case "2" : p.GetAccountTransactions(testAccount); break;
                    case "3" : testMosaic = p.CreateMosaicTest(testAccount); break;
                    case "4" : p.ModifyMosaicSupplyTest(testAccount, testMosaic); break;
                    case "5" : p.SendMosaicTest(testAccount, testMosaic); break;
                    case "6" : testNamespace = p.CreateNamespaceTest(testAccount); break;   //TODO make this use the test account to sign
                    case "7" : p.CreateSubNamespaceTest(testAccount); break;           //TODO make this use the test account to sign
                    //case "8" : p.LinkMosaicToNamespaceTest(testAccount, testMosaic); break;   //TODO Link it with created mosaic
                }
                Console.WriteLine("\nPress any key to proceed..");
                System.Console.ReadKey();
            }

            //p.CreateAccountTest(0); //Creates a new account and sends xpx from BSG_1 to the new account
            //p.SendXPXTest();
            //p.GetAccountTransactions(); //Gets all transactions from account
            //p.CreateMosaicTest();
            //p.SendMosaicTest();
            //p.CreateNamespace();
            //p.CreateSubNamespace();
            //p.LinkMosaicToNamespaceTest();

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
            XarcadeParams.CreateNamespaceParams param = new XarcadeParams.CreateNamespaceParams();
            param.accountDTO = pAccount.CreateAccount(1, newAccount.PrivateKey);
            param.Domain = namespaceName;
            XarcadeModels.NamespaceDTO createNamespaceT = pNamespace.CreateNamespaceAsync(param).GetAwaiter().GetResult();

            Console.WriteLine(createNamespaceT.ToString());
            return createNamespaceT;
        }

        //Creates a subnamespace using Bruh's private key and Parent namespace
        private XarcadeModels.NamespaceDTO CreateSubNamespaceTest(XarcadeModels.AccountDTO newAccount)
        {
            XarcadeParams.CreateNamespaceParams param = new XarcadeParams.CreateNamespaceParams();
            param.accountDTO = pAccount.CreateAccount(1, newAccount.PrivateKey);
            param.Domain = namespaceName;
            param.LayerOne = subNamespaceName;

            XarcadeModels.NamespaceDTO createNamespaceT = pNamespace.CreateSubNamespaceAsync(param).GetAwaiter().GetResult();
            
            Console.WriteLine(createNamespaceT.ToString());
            return createNamespaceT;
        }
        
        private void LinkMosaicToNamespaceTest()
        {
            XarcadeParams.LinkMosaicParams param = new XarcadeParams.LinkMosaicParams
            {
                accountDTO   =  pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1),
                mosaicID     = pMosaic.GetMosaicAsync("798b57c1850c523c").GetAwaiter().GetResult().MosaicID,
                namespaceDTO = pNamespace.GetNamespaceInformation("linktest").GetAwaiter().GetResult()
            };

            var link = pMosaic.LinkMosaicAsync(param).GetAwaiter().GetResult();
        }


    }
}
