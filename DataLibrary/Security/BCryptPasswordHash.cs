using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Security
{
    public class BCryptPasswordHash : IPasswordHash
    {
        public bool CheckPassword(string plaintext, string hashed)
        {
            return BCrypt.CheckPassword(plaintext, hashed);
        }

        public string HashPassword(string plaintext)
        {
            return BCrypt.HashPassword(plaintext, BCrypt.GenerateSalt());
        }
    }
}
