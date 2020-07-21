using System;
using System.Threading.Tasks;
using Xarcade.Domain.ProximaX;
using Xarcade.Infrastructure.ProximaX.Params;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities.Logger;
using Xarcade.Application.Xarcade.Models.Account;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Application.Xarcade.Models.Transaction;


namespace Xarcade.Application.Xarcade
{
   
    public class TransactionService
    {
        public readonly IBlockchainPortal blockchainPortal;
        public readonly IDataAccessProximaX dataAccessProximaX;
        public static ILogger _logger;

        public TransactionService(IDataAccessProximaX dataAccessProximaX, IBlockchainPortal blockchainPortal)
        {
            this.dataAccessProximaX = dataAccessProximaX;
            this.blockchainPortal = blockchainPortal;
        }
        /// <summary>
        /// Sends a custom token from user to another user
        /// </summary>
        public async Task<TokenTransactionDto> SendTokenAsync(TokenDto token,AccountDto sender, AccountDto receiver)
        {
            var SenderUserDB = dataAccessProximaX.LoadUser(sender.UserID); // account from database
            var ReceiverUserDB = dataAccessProximaX.LoadUser(receiver.UserID); //account from database
 
            var param = new SendMosaicParams
            {
            MosaicID = token.TokenId, 
            Sender = SenderUserDB,
            RecepientAddress = ReceiverUserDB.WalletAddress,
            Amount =  token.Quantity,
            Message = "PloxWork.jpeg"
            };

            Transaction sendtokentransaction = blockchainPortal.SendMosaicAsync(param).GetAwaiter().GetResult(); //Update Blockchain
            dataAccessProximaX.SaveTransaction(sendtokentransaction); //Save Transaction in mongodb
            
            var tokentransactiondto = new TokenTransactionDto
            {
                Status = 0, //How to determine status?
                Hash = sendtokentransaction.Hash,
                Token = token,
                BlockNumber = 0, // Where to find the block number?
                Created = sendtokentransaction.Created,
            };
            

            return await Task.FromResult<TokenTransactionDto>(null);
        }

        /// <summary>
        /// Sends a Xar token from user to another user
        /// </summary>
        public async Task<TokenTransactionDto> SendXarAsync(TokenDto token,AccountDto sender, AccountDto receiver)
        {
            var SenderUserDB = dataAccessProximaX.LoadUser(sender.UserID); // account from database
            var ReceiverUserDB = dataAccessProximaX.LoadUser(receiver.UserID); //account from database
 
            var param = new SendMosaicParams
            {
            MosaicID = token.TokenId, 
            Sender = SenderUserDB,
            RecepientAddress = ReceiverUserDB.WalletAddress,
            Amount =  token.Quantity,
            Message = "PloxWork.jpeg"
            };

            Transaction sendxartransaction = blockchainPortal.SendMosaicAsync(param).GetAwaiter().GetResult(); //Update Blockchain
            dataAccessProximaX.SaveTransaction(sendxartransaction); //Save Transaction in mongodb
            
            var tokentransactiondto = new TokenTransactionDto
            {
                Status = 0, //How to determine status?
                Hash = sendxartransaction.Hash,
                Token = token,
                BlockNumber = 0, // Where to find the block number?
                Created = sendxartransaction.Created,
            };
            return await Task.FromResult<TokenTransactionDto>(null);
        }
        /// <summary>
        /// Sends a Xpx token from user to another user
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public async Task<TokenTransactionDto> SendXpxAsync(TokenDto token,AccountDto sender, AccountDto receiver)
        {
            var SenderUserDB = dataAccessProximaX.LoadUser(sender.UserID); // account from database
            var ReceiverUserDB = dataAccessProximaX.LoadUser(receiver.UserID); //account from database


            var param = new SendXpxParams();
            param.Sender = SenderUserDB;
            param.RecepientAddress = ReceiverUserDB.WalletAddress;
            param.Amount =  token.Quantity ;
            param.Message = "PloxWork.jpeg";

            Transaction sendxpxtransaction = blockchainPortal.SendXPXAsync(param).GetAwaiter().GetResult(); //Update Blockchain
            dataAccessProximaX.SaveTransaction(sendxpxtransaction); //Save Transaction in mongodb
            
            var tokentransactiondto = new TokenTransactionDto
            {
                Status = 0, //How to determine status?
                Hash = sendxpxtransaction.Hash,
                Token = token,
                BlockNumber = 0, // Where to find the block number?
                Created = sendxpxtransaction.Created,
            };
            
            return await Task.FromResult<TokenTransactionDto>(null);
        }
    }

}