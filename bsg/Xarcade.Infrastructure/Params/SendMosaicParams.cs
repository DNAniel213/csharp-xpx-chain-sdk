using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class SendMosaicParams
    {
        /// <summary> The mosaic being sent. </summary>
        public string MosaicID { get; set; } = null;

        /// <summary> The account sending the mosaic. </summary>
        public Account Sender { get; set; } = null;

        /// <summary> The wallet address receiving the mosaic. </summary>
        public string RecipientAddress { get; set; } = null;

        /// <summary> The amout of mosaic being sent. </summary>
        public ulong Amount { get; set; } = 0;

        /// <summary> The message when sending mosaic. Default is null. </summary>
        public string Message { get; set; } = null;
    }

}