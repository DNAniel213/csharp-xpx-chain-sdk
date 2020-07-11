using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System;

namespace Xarcade.Api.Prototype.Cryptography
{
    public class ApiKeyServices
    {
        public static string GenerateRandomString()
        {
            char[] letters = "qwertyuiopasdfghjklzxcvbnm1234567890".ToCharArray();
            Random r = new Random();
            string randomString = "";
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    randomString += letters[r.Next(0,35)].ToString();
                }
            }
            catch(Exception)
            {
                return null;
            }
            return randomString;
        }

        public static string DecodeFrom64(string encodedData)
        {
            if(encodedData != null)
            {
                throw new System.ArgumentException("Parameter is required: encoded string");
            }
            else
            {
                try
                {
                    var toDecodeDataAsString = System.Convert.FromBase64String(encodedData);
                    return System.Text.ASCIIEncoding.ASCII.GetString(toDecodeDataAsString);
                }
                catch(Exception)
                {
                    return null;
                }
            }
        }

        public static string EncodeTo64(string toEncode)
        {
            if(toEncode == null)
            {
                throw new System.ArgumentException("Parameter is required: string to encode.");
            }
            else
            {
                try
                {
                    var toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
                    return System.Convert.ToBase64String(toEncodeAsBytes);
                }
                catch(Exception)
                {
                    return null;
                }
            }
        }
    }
}