using System.Text;
using System.Convert;

namespace Xarcade.Api.Prototype.Cryptography
{
    public class EncryptionServices
    {
        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }
    }
}