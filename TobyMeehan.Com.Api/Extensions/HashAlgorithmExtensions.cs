using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Extensions
{
    public static class HashAlgorithmExtensions
    {
        public static string ComputeCodeChallenge(this HashAlgorithm hash, string codeVerifier)
        {
            string challenge = Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier)));

            challenge = (challenge.EndsWith("=") ? challenge.Remove(challenge.LastIndexOf("=")) : challenge).Replace("+", "-").Replace("/", "_");

            return challenge;
        }
    }
}
