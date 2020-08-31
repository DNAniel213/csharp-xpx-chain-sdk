using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class SendMosaicParams
    {
        public string MosaicID { get; set; } = null;
        /// <summary>
        /// Account to send the mosaic from
        /// </summary>
        public Account Sender { get; set; } = null;
        public string RecepientAddress { get; set; } = null;
        public ulong Amount { get; set; } = 0;
        public string Message { get; set; } = null;
    }

}