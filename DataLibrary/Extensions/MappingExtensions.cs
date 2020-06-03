using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Extensions
{
    internal static class MappingExtensions
    {
        public static User Map(this User user, Role role, Transaction transaction)
        {
            user.Roles = user.Roles ?? new List<Role>();
            user.Transactions = user.Transactions ?? new List<Transaction>();

            user.Roles.Add(role);
            user.Transactions.Add(transaction);

            return user;
        }

        public static Application Map(this Application app, User user, Role role, Transaction transaction)
        {
            app.Author = app.Author ?? user;

            app.Author = user.Map(role, transaction);

            return app;
        }
    }
}
