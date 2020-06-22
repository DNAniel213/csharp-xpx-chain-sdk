using System;
using Xarcade.Domain.Models;

namespace Xarcade.Domain.Params
{
    public class SendMosaicParams
    {
        public ulong mosaicID = 0;
        /// <summary>
        /// Account to send the mosaic from
        /// </summary>
        public AccountDTO sender = null;
        public string recepientAddress = null;
        public ulong amount = 0;
        public string message = null;
    }

}