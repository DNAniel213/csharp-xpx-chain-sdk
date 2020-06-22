using Xarcade.Domain.Models;
using System;

namespace Xarcade.Domain.Params
{
    public class SendXpxParams
    {
        /// <summary>
        /// Account to send xpx from
        /// </summary>
        public AccountDTO sender = null;
        public string recepientAddress = null;
        public ulong amount = 0;
        public string message = null;
    }

}