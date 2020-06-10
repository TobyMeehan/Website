using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Extensions
{
    public static class UserExtensions
    {
        public static bool IsVerified(this User user)
        {
            return user.Roles.Any(r => r.Name == UserRoles.Verified);
        }
    }
}
