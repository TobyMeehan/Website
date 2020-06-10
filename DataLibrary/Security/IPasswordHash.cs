using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Security
{
    public interface IPasswordHash
    {
        string HashPassword(string plaintext);

        bool CheckPassword(string plaintext, string hashed);
    }
}
