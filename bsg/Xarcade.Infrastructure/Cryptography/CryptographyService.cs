using System;
using System.Security.Cryptography;
using Xarcade.Infrastructure.ProximaX;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Utilities.Logger;
using Xarcade.Infrastructure.Repository;

namespace Xarcade.Infrastructure.Cryptography
{
    public class CryptographyService : ICryptographyService
    {
        private readonly IDataAccessProximaX dataAccessProximaX;
        private RSACryptoServiceProvider _RSAService = new RSACryptoServiceProvider(2048);
        private static ILogger _logger;

        public string Encrypt(string text)
        {
            DataAccessProximaX dataAccessProximaX = new DataAccessProximaX();

            if(text == null)
            {
                _logger.LogError("parameter is empty");
            }
            var privKey = _RSAService.ExportParameters(true);
            var pubKey = _RSAService.ExportParameters(false);
            string pubKeyString;
                {
                    var buffer = new System.IO.StringWriter();
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    serializer.Serialize(buffer, pubKey);
                    pubKeyString = buffer.ToString();
                }
            string privKeyString;
                {
                    var buffer = new System.IO.StringWriter();
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    serializer.Serialize(buffer, privKey);
                    privKeyString = buffer.ToString();
                }

            try
            {
                var stream = new System.IO.StringReader(pubKeyString);
                var deserializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                pubKey = (RSAParameters)deserializer.Deserialize(stream);

                RSACryptoServiceProvider loadPublicKey = new RSACryptoServiceProvider();
                loadPublicKey.ImportParameters(pubKey);
                
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
            }
            
            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(text);

            var bytesCypherText = _RSAService.Encrypt(bytesPlainTextData, false);

            var cypherText = Convert.ToBase64String(bytesCypherText);

            Keys keys = new Keys
            {
                PrivateKey  = privKeyString,
                PublicKey   = pubKeyString,
                TheText     = cypherText,
            };
            
            dataAccessProximaX.SaveKeys(keys);

            return cypherText;
        }
        public string Decrypt(string text)
        {
            
            DataAccessProximaX dataAccessProximaX = new DataAccessProximaX();

            if(text == null)
            {
                _logger.LogError("parameter is empty");
            }

            var keysString = dataAccessProximaX.LoadKeys(text);
            var bytesCypherText = Convert.FromBase64String(text);

            var stream = new System.IO.StringReader(keysString.PrivateKey);
            var deserializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            var privKey = (RSAParameters)deserializer.Deserialize(stream);
            
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privKey);

            var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

            var plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

            return plainTextData;
            
        }

    }
}