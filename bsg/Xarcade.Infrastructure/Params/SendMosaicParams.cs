using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class SendMosaicParams
    {
        public ulong MosaicID = 0;
        /// <summary>
        /// Account to send the mosaic from
        /// </summary>
        public AccountDTO Sender { get; set; } = null;
        public string RecepientAddress { get; set; } = null;
        public ulong Amount { get; set; } = 0;
        public string Message { get; set; } = null;
    }

}