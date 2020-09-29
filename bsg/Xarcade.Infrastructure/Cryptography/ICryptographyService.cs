
namespace Xarcade.Infrastructure.ProximaX
{
    public interface ICryptographyService
    {
        /// <summary>
        /// Encrypts the privateKey using RSA
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        string Encrypt(string privateKey);

        /// <summary>
        /// Decrypts the encrypted key to string
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        string Decrypt(string privateKey);
    }
}