using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class SendXpxParams
    {
        /// <summary> The account sending XPX. </summary>
        public Account Sender {get; set;} = null;

        /// <summary> The wallet address receiving XPX. </summary>
        public string RecipientAddress {get; set;} = null;

        /// <summary> The amount of XPX being sent. </summary>
        public ulong Amount {get; set;} = 0;

        /// <summary> The message when sending XPX. Default is null. </summary>
        public string Message {get; set;} = null;
    }

}