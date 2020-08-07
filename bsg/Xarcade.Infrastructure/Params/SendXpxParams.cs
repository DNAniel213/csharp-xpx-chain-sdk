using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class SendXpxParams
    {
        /// <summary>
        /// Account to send xpx from
        /// </summary>
        public Account Sender {get; set;} = null;
        public string RecepientAddress {get; set;} = null;
        public ulong Amount {get; set;} = 0;
        public string Message {get; set;} = null;
        public long AssetID {get; set;} = 0;
    }

}