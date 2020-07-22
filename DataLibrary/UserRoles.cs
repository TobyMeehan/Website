using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data
{
    public class UserRoles
    {
        public const string Admin = "Admin";

        public const string Verified = "Verified";

        // Button Roles
        public const string Purple = nameof(Purple);
        public const string Blue = nameof(Blue);
        public const string Green = nameof(Green);
        public const string Yellow = nameof(Yellow);
        public const string Orange = nameof(Orange);
        public const string Red = nameof(Red);

        public static IEnumerable<string> ButtonRoles => new[] { Purple, Blue, Green, Yellow, Orange, Red };
    }
}
