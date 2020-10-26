using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ProbaDotnetSDK.Services
{
    internal class HmacService
    {
        public string GenerateHmacSignature(string secretKey, string message)
        {
            using (var hmacsha256 = new HMACSHA256(secretKey.FromUTF8()))
            {
                byte[] hashedmessage = hmacsha256.ComputeHash(message.FromUTF8());
                return hashedmessage.ToBase64String();
            }

        }
    }
}
