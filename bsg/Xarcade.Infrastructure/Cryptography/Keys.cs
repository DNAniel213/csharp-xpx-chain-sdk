using System.Security.Cryptography;

namespace Xarcade.Infrastructure.Cryptography
{
    public class Keys
    {
        public string TheText;
        public string PublicKey;
        public string PrivateKey;

        public override string ToString()
        {
            return
                "===Game DTO==="    +
                "\nTheText: "       + TheText +
                "\nPublic Key: "    + PublicKey +
                "\nPrivate Key: "   + PrivateKey +
                "\n==End of Game DTO==";
        }
    }
}