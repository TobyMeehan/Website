using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Extensions
{
    public static class GuidExtensions
    {
        public static string ToToken(this Guid guid)
        {
            return guid.ToString().Replace("-", "");
        }
    }
}
