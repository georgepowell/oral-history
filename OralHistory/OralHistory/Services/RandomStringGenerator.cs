using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace OralHistory.Services
{
    static class RandomStringGenerator
    {
        public static string GetRandomString(int length, params char[] chars)
        {
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
            string s = "";
            for (int i = 0; i < length; i++)
            {
                byte[] intBytes = new byte[4];
                rand.GetBytes(intBytes);
                uint randomInt = BitConverter.ToUInt32(intBytes, 0);
                s += chars[randomInt % chars.Length];
            }
            return s;
        }
    }
}