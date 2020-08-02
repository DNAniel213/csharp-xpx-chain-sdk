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
            //1.Check if user exist.
            if(sender == null || receiver == null)
            {
                _logger.LogError("A user does not exist!!");
                return null;
            }
            //2. Check if Xpx balance of sender is enough since sending mosaics I think has a fee.
            // Need Function Implementation to getAccountInfo from BlockChainPortal
            // No function currently exists returning AccountInfo

            //3. Check if token is enough against the amount to be sent.
            var tokenBalance = await blockchainPortal.GetMosaicAsync(token.TokenId);
            if (tokenBalance.Quantity < token.Quantity )
            {
                _logger.LogError("Not enough tokens to send!!");
                return null;
            }

            var SenderUserDB = dataAccessProximaX.LoadUser(sender.UserID); // account from database
            var ReceiverUserDB = dataAccessProximaX.LoadUser(receiver.UserID); //account from database

            //check return from DB if null or not then return error
            if (SenderUserDB == null || ReceiverUserDB == null)
            {
                _logger.LogError("A account does not exist!!");
                return null;
            }
 
            var param = new SendMosaicParams
            {
                MosaicID = token.TokenId, 
                Sender = SenderUserDB,
                RecepientAddress = ReceiverUserDB.WalletAddress,
                Amount =  token.Quantity,
                Message = "PloxWork.jpeg"
            };

            Transaction sendtokentransaction = await blockchainPortal.SendMosaicAsync(param); //Update Blockchain

            if (sendtokentransaction == null)
            {
                _logger.LogError("The transaction does not exist.");
                return null;
            }

            dataAccessProximaX.SaveTransaction(sendtokentransaction); //Save Transaction in mongodb
            
            Transaction transactioninfo = await blockchainPortal.GetTransactionInformationAsync(sendtokentransaction.Hash);

            var tokentransactiondto = new TokenTransactionDto
            {
                Status = 0, // @Janyl Implement changes BlockChainPortal.transaction to return transactioninfo
                Hash = sendtokentransaction.Hash,
                Token = token,
                BlockNumber = sendtokentransaction.Height, 
                Created = sendtokentransaction.Created,
            };

            return tokentransactiondto;
        }

        /// <summary>
        /// Sends a Xar token from user to another user
        /// </summary>
        public async Task<TokenTransactionDto> SendXarAsync(TokenDto token,AccountDto sender, AccountDto receiver)
        {
            //1.Check if user exist.
            if(sender == null || receiver == null)
            {
                _logger.LogError("A user does not exist!!");
                return null;
            }
            //2. Check if Xpx balance of sender is enough since sending mosaics I think has a fee.
            // Need Function Implementation to getAccountInfo from BlockChainPortal
            // No function currently exists returning AccountInfo

            //3. Check if token is enough against the amount to be sent.
            var tokenBalance = await blockchainPortal.GetMosaicAsync(token.TokenId);
            if (tokenBalance.Quantity < token.Quantity )
            {
                _logger.LogError("Not enough tokens to send!!");
                return null;
            }
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

            Transaction sendxartransaction = await blockchainPortal.SendMosaicAsync(param); //Update Blockchain
            
            if (sendxartransaction == null)
            {
                _logger.LogError("The transaction does not exist.");
                return null;
            }

            dataAccessProximaX.SaveTransaction(sendxartransaction); //Save Transaction in mongodb
            
            var tokentransactiondto = new TokenTransactionDto
            {
                Status = 0, // @Janyl Implement changes BlockChainPortal.transaction to return transactioninfo
                Hash = sendxartransaction.Hash,
                Token = token,
                BlockNumber = sendxartransaction.Height, // @Janyl Implement changes to transaction model in Xarcade.domain
                Created = sendxartransaction.Created,
            };
            return tokentransactiondto;
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
            //1.Check if user exist.
            if(sender == null || receiver == null)
            {
                _logger.LogError("A user does not exist!!");
                return null;
            }
            //2. Check if Xpx balance of sender is enough since sending mosaics I think has a fee.
            // Need Function Implementation to getAccountInfo from BlockChainPortal
            // No function currently exists returning AccountInfo

            //3. Check if token is enough against the amount to be sent.
            var tokenBalance = await blockchainPortal.GetMosaicAsync(token.TokenId);
            if (tokenBalance.Quantity < token.Quantity )
            {
                _logger.LogError("Not enough tokens to send!!");
                return null;
            }
            var SenderUserDB = dataAccessProximaX.LoadUser(sender.UserID); // account from database
            var ReceiverUserDB = dataAccessProximaX.LoadUser(receiver.UserID); //account from database

            //check return from DB if null or not then return error
            if (SenderUserDB == null || ReceiverUserDB == null)
            {
                _logger.LogError("An account does not exist!!");
                return null;
            }
            var param = new SendXpxParams
            {
                Sender = SenderUserDB,
                RecepientAddress = ReceiverUserDB.WalletAddress,
                Amount =  token.Quantity,
                Message = "PloxWork.jpeg"
            };

            Transaction sendxpxtransaction = await blockchainPortal.SendXPXAsync(param); //Update Blockchain
            
            if (sendxpxtransaction == null)
            {
                _logger.LogError("The transaction does not exist.");
                return null;
            }

            dataAccessProximaX.SaveTransaction(sendxpxtransaction); //Save Transaction in mongodb
            
            var tokentransactiondto = new TokenTransactionDto
            {
                Status = 0, // @Janyl Implement changes BlockChainPortal.transaction to return transactioninfo
                Hash = sendxpxtransaction.Hash,
                Token = token,
                BlockNumber = 0, // @Janyl Implement changes to transaction model in Xarcade.domain
                Created = sendxpxtransaction.Created,
            };
            
            return tokentransactiondto;
        }
    }

}