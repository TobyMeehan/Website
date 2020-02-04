using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DataAccessLibrary.Security
{
    /// <summary>
    /// Generator for API client secrets.
    /// </summary>
    class ClientSecret
    {
        /// <summary>
        /// Generates a secure random string for use as a client secret.
        /// </summary>
        /// <param name="length">Length of secret. Defaults to 32.</param>
        /// <returns></returns>
        public static string Generate(int length = 32)
        {
            RandomNumberGenerator cryptoRandomDataGenerator = new RNGCryptoServiceProvider();

            byte[] buffer = new byte[length];

            cryptoRandomDataGenerator.GetBytes(buffer);

            string secret = Convert.ToBase64String(buffer);

            return secret;
        }
    }
}
