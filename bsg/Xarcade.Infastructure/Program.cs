using System.Collections.Generic;
using System;
using XarcadeModel = Xarcade.Domain.Models;
using Xarcade.Api.Prototype.Blockchain;

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

            //p.CreateNewAccountAndSendXpx(); //Creates a new account and sends xpx from BSG_1 to the new account
            //p.GetAccountTransactions(); //Gets all transactions from account
            //p.CreateMosaicTest();
            //p.SendMosaicTest();
            //p.CreateNamespace();
            //p.CreateSubNamespace();
            //p.LinkMosaicToNamespaceTest();
            //p.ExtendNamespaceDuration();
            p.CreateNewAccountAndSendXpx();
        }

        //read the function name
        private void CreateNewAccountAndSendXpx()
        {
            string message = "Hey! Alright! Buy a shirt, will ya?~";
            XarcadeModel.AccountDTO newAccount = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1);
            XarcadeModel.AccountDTO dane2 = pAccount.CreateAccount(2, TEST_PRIVATE_BSG_2);
            XarcadeModel.TransactionDTO sendCurrency = pTransaction.SendXPXAsync(newAccount, dane2.WalletAddress, 20, message).GetAwaiter().GetResult();

            Console.WriteLine(newAccount.PublicKey);
            // Verifies whether the new account had 20 mosaic units
            //var newBalance = pAccount.GetAccountInformation(newAccount.PrivateKey).GetAwaiter().GetResult();
            //Console.WriteLine($"{nameof(newBalance)} : {newBalance}");
        }

        private void GetAccountTransactions()
        {
            List<XarcadeModel.TransactionDTO> transactions = pAccount.GetAccountTransactions(TEST_PRIVATE_BSG_1, 10).GetAwaiter().GetResult();
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
            XarcadeModel.AccountDTO account = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1); 

            XarcadeModel.MosaicDTO createMosaicT = pMosaic.CreateMosaicAsync(account, true, true, false, 0, 1000).GetAwaiter().GetResult(); 
            XarcadeModel.MosaicDTO supplyChangeT = pMosaic.ModifyCoinSupply(account, createMosaicT.MosaicID, 50000).GetAwaiter().GetResult();
                        
        }

        //Sends mosaic from Dane1 to Dane2 accounts. Usually, only need PUBLIC KEY from recepient account 
        private void SendMosaicTest()
        {
            XarcadeModel.AccountDTO dane1 = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1); 
            XarcadeModel.AccountDTO dane2 = pAccount.CreateAccount(2, TEST_PRIVATE_BSG_2); 
            XarcadeModel.MosaicDTO mosaicInfo = pMosaic.GetMosaicAsync("58df4e1d10799100").GetAwaiter().GetResult();

            XarcadeModel.TransactionDTO sendCoinT = pMosaic.SendMosaicAsync(mosaicInfo.MosaicID, dane1, dane2.WalletAddress, 20, "hope this works.jpg").GetAwaiter().GetResult();
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
        
        //Creates a link between the 'linktest' Namespace and a Mosaic
        /*
        private void LinkMosaicToNamespaceTest()
        {
            XarcadeModel.AccountDTO account = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1);
            XarcadeModel.NamespaceDTO namespaceInfo = pNamespace.GetNamespaceInformation("linktest").GetAwaiter().GetResult();
            XarcadeModel.MosaicDTO mosaicInfo = pMosaic.GetMosaicAsync("798b57c1850c523c").GetAwaiter().GetResult();

            var link = pMosaic.LinkMosaicAsync(account, mosaicInfo.MosaicID, namespaceInfo).GetAwaiter().GetResult();
        }*/

        //Extends the duration of a rented Namespace using the name of the namespace, user private key, namespace info, and desired duration for extension
        private void ExtendNamespaceDuration()
        {
            ulong? days = 65;//take note of the remaining duration of the namespace | 365 days max
            ulong? duration = days * 86400/15;

            var account = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1); //AccountDTO
            //Account account = Account.CreateFromPrivateKey(accountDTO.PrivateKey, portal.networkType);
            var namespaceInfo = pNamespace.GetNamespaceInformation(namespaceName).GetAwaiter().GetResult();
            var extendNamespace = pNamespace.ExtendNamespaceDuration(namespaceName,TEST_PRIVATE_BSG_1,namespaceInfo,duration);
            //portal.SignAndAnnounceTransaction(account, extendNamespace).GetAwaiter().GetResult();
        }


    }
}
