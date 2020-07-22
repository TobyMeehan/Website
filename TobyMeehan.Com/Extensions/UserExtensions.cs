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

        public static bool TryGetButtonRole(this User user, out Role buttonRole)
        {
            buttonRole = user.Roles.FirstOrDefault(r => UserRoles.ButtonRoles.Contains(r.Name));

            return buttonRole != null;
        }
    }
}
