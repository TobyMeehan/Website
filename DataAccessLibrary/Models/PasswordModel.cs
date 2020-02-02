using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class PasswordModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string HashedPassword { get; set; }

    }
}
