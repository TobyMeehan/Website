using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DataAccessLibrary.Security
{
    class Pkce
    {
        /// <summary>
        /// Returns the code challenge corresponding to the provided verifier (SHA256 hash, replace + with -, replace / with _, trim trailing =)
        /// </summary>
        /// <param name="codeVerifier"></param>
        /// <returns></returns>
        public static string ChallengeFromVerifier(string codeVerifier)
        {
            string challenge = Convert.ToBase64String(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(codeVerifier)));

            challenge = (challenge.EndsWith("=") ? challenge.Remove(challenge.LastIndexOf("=")) : challenge).Replace("+", "-").Replace("/", "_");

            return challenge;
        }
    }
}
