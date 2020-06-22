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
        static void Main(string[] args)
        {
            Program p = new Program();
            p.portal = new ProximaxBlockchainPortal();
            p.pAccount = new ProximaxAccount(p.portal);
            p.pMosaic = new ProximaxMosaic(p.portal);
            p.pNamespace = new ProximaxNamespace(p.portal);
            p.pTransaction = new ProximaxTransaction(p.portal);

            
            //var client = new MongoClient("mongodb+srv://dane:pikentz213@xarcadeprotodb-ctyys.mongodb.net/<dbname>?retryWrites=true&w=majority");
            //var database = client.GetDatabase("test");


            p.CreateAccountTest(0); //Creates a new account and sends xpx from BSG_1 to the new account
            //p.SendXPXTest();
            //p.GetAccountTransactions(); //Gets all transactions from account
            //p.CreateMosaicTest();
            //p.SendMosaicTest();
            //p.CreateNamespace();
            //p.CreateSubNamespace();
            //p.LinkMosaicToNamespaceTest();

        }

        //read the function name
        private void CreateAccountTest(int id)
        {
            XarcadeModels.AccountDTO newAccount = pAccount.CreateAccount(id);
            Console.WriteLine(newAccount.ToString());


        }

        private void SendXPXTest(XarcadeModels.AccountDTO newAccount)
        {   
            XarcadeModels.AccountDTO dane2 = pAccount.CreateAccount(2, TEST_PRIVATE_BSG_2);
            XarcadeParams.SendXpxParams param = new XarcadeParams.SendXpxParams();
            param.sender = newAccount;
            param.recepientAddress = dane2.WalletAddress;
            param.amount = 53;
            param.message = "Pls work.jpg";


            XarcadeModels.TransactionDTO sendCurrency = pTransaction.SendXPXAsync(param).GetAwaiter().GetResult();
            // TODO Add tracking here
        }

        private void GetAccountTransactions()
        {
            List<XarcadeModels.TransactionDTO> transactions = pAccount.GetAccountTransactions(TEST_PRIVATE_BSG_1, 10).GetAwaiter().GetResult();
            int i = 0;
            foreach (var tx in transactions)
            {
                Console.WriteLine("tx" + i + " Sender: " + tx.Hash +
                                    " CreatedOn: " + tx.Created);
                i++;
            }


        }


        //Creates a mosaic using Dane's private key
        private void CreateMosaicTest()
        {
            XarcadeParams.CreateMosaicParams param = new XarcadeParams.CreateMosaicParams();
            param.accountDTO = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1); 
            XarcadeModels.MosaicDTO createMosaicT = pMosaic.CreateMosaicAsync(param).GetAwaiter().GetResult(); 
            // TODO Add tracking here

        }

        private void ModifyMosaicSupplyTest()
        {
            XarcadeParams.ModifyMosaicSupplyParams param = new XarcadeParams.ModifyMosaicSupplyParams
            {
                accountDTO = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1),
                mosaicID = "58df4e1d10799100",
                amount = 10000
            };
            XarcadeModels.MosaicDTO modifyMosaicT = pMosaic.ModifyMosaicSupply(param).GetAwaiter().GetResult();
            // TODO Add tracking here

        }

        //Sends mosaic from Dane1 to Dane2 accounts. Usually, only need PUBLIC KEY from recepient account 
        private void SendMosaicTest()
        {
            XarcadeParams.SendMosaicParams param = new XarcadeParams.SendMosaicParams
            {
                mosaicID = pMosaic.GetMosaicAsync("58df4e1d10799100").GetAwaiter().GetResult().MosaicID,
                sender = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1),
                recepientAddress = pAccount.CreateAccount(2, TEST_PRIVATE_BSG_2).WalletAddress,
                amount = 59,
                message = "Hope this works"

            };

            XarcadeModels.TransactionDTO sendCoinT = pMosaic.SendMosaicAsync(param).GetAwaiter().GetResult();
        }

        //Creates a namespace using Bruh's Private Key
    
        private void CreateNamespace()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1);

            try
            {
                var namespaceInfo = pNamespace.GetNamespaceInformation(namespaceName).GetAwaiter().GetResult();
                throw new ArgumentNullException($"Namespace already exists!");
            }
            catch (Flurl.Http.FlurlHttpException)
            {
                var registerNamespace = pNamespace.CreateNamespace(namespaceName, 100); //Dapat naa na sa sulod daan ang signing
            }
        }

        //Creates a subnamespace using Bruh's private key and Parent namespace
        private void CreateSubNamespace()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1);

            var subNamespaceName = "wahwah";

            var registerSubNamespace = pNamespace.CreateSubNamespace(subNamespaceName, namespaceName); //Dapat naa na sa sulod ang pag sign
        }
        
        private void LinkMosaicToNamespaceTest()
        {
            XarcadeParams.LinkMosaicParams param = new XarcadeParams.LinkMosaicParams
            {
                accountDTO   =  pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1),
                mosaicID     = pMosaic.GetMosaicAsync("798b57c1850c523c").GetAwaiter().GetResult().MosaicID,
                namespaceDTO = pNamespace.GetNamespaceInformation("linktest").GetAwaiter().GetResult()
            };
            //TODO CHANGE THIS TO param version

            //var link = pMosaic.LinkMosaicAsync(account, mosaicInfo.MosaicID, namespaceInfo).GetAwaiter().GetResult();
        }


    }
}
