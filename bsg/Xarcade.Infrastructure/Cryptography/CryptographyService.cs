using System;
using System.Security.Cryptography;
using Xarcade.Infrastructure.ProximaX;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Utilities.Logger;

namespace CryptographyService
{
    public class CryptographyService : ICryptographyService
    {
        private readonly IDataAccessProximaX dataAccessProximaX;
        private readonly RSACryptoServiceProvider _RSAService = new RSACryptoServiceProvider(2048);
        private static ILogger _logger;

        public string Encrypt(string privateKey)
        {
            if(privateKey == null)
            {
                _logger.LogError("parameter is empty");
            }

            try
            {
                var privKey = _RSAService.ExportParameters(true);

                var pubKey = _RSAService.ExportParameters(false);

                string pubKeyString;
                {
                    var buffer = new System.IO.StringWriter();
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    serializer.Serialize(buffer, pubKey);
                    pubKeyString = buffer.ToString();
                }

                {
                    var stream = new System.IO.StringReader(pubKeyString);
                    var deserializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    pubKey = (RSAParameters)deserializer.Deserialize(stream);
                }


                RSACryptoServiceProvider loadPublicKey = new RSACryptoServiceProvider();
                loadPublicKey.ImportParameters(pubKey);
                
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
            }

            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(privateKey);

            var bytesCypherText = _RSAService.Encrypt(bytesPlainTextData, false);

            var cypherText = Convert.ToBase64String(bytesCypherText);

            return cypherText;
        }
        public string Decrypt(string privateKey)
        {
            if(privateKey == null)
            {
                _logger.LogError("parameter is empty");
            }
            try
            {
                var privKey = _RSAService.ExportParameters(true);

                var pubKey = _RSAService.ExportParameters(false);

                string pubKeyString;
                {
                    var buffer = new System.IO.StringWriter();
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    serializer.Serialize(buffer, pubKey);
                    pubKeyString = buffer.ToString();
                }

                {
                    var stream = new System.IO.StringReader(pubKeyString);
                    var deserializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    pubKey = (RSAParameters)deserializer.Deserialize(stream);
                }

                RSACryptoServiceProvider loadPrivateKey = new RSACryptoServiceProvider();
                loadPrivateKey.ImportParameters(privKey);
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
            }
            var bytesCypherText = Convert.FromBase64String(privateKey);

            var bytesPlainTextData = _RSAService.Decrypt(bytesCypherText, false);

            var plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

            return plainTextData;
        }

    }
}