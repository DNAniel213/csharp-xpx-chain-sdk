using System.Text;
using System.Convert;

namespace Xarcade.Api.Prototype.Cryptography
{
    public class DecryptionServices
    {        
        static public string DecodeFrom64(string encodedData)
        {
            byte[] toEncodeDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(toEncodeAsBytes);

            return returnValue;
        }
    }
}