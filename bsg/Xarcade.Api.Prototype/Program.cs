using System;
using XarcadeAccount = Xarcade.Api.Prototype.Blockchain.Models;
using Xarcade.Api.Prototype.Blockchain;
namespace Xarcade.Api.Prototype
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            ProximaxBlockchainPortal portal = new ProximaxBlockchainPortal();
            ProximaxAccount pAccount = new ProximaxAccount(portal);
            ProximaxMosaic pMosaic = new ProximaxMosaic(portal);

            XarcadeAccount.Account account = pAccount.CreateAccount(1);
            var mosaic = pMosaic.CreateCurrency(account, true, true, true, 0, 1000); 
        }
    }
}
