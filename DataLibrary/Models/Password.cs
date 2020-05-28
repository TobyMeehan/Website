using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Data.Models
{
    public class Password
    {
		public Password(string plaintext)
		{
			PlainText = plaintext;
		}

		private string _plaintext;
		public string PlainText
		{
			get { return _plaintext; }

			set 
			{
				Hashed = BCrypt.HashPassword(value, BCrypt.GenerateSalt());
				_plaintext = value;
			}
		}

		public string Hashed { get; private set; }

		public static implicit operator string(Password password) => password.Hashed;
	}
}
