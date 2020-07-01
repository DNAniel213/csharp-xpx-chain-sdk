using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Xarcade.Api.Prototype.Cryptography
{
    public class GenerateRandomString
    {
        static public string GenerateRandom()
        {
            char[] letters = "qwertyuiopasdfghjklzxcvbnm1234567890".ToCharArray();
            Random r = new Random();
            string randomString = "";
            
            for (int i = 0; i < 10; i++)
            {
                randomString += letters[r.Next(0,35)].ToString();
            }
            
            return randomString;
        }
    }
}
