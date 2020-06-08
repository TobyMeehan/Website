using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Extensions
{
    internal static class MappingExtensions
    {
        public static User Map(this User user, Role role, Transaction transaction)
        {
            if (!user.Roles.TryGetItem(role?.Id, out Role roleEntity) && role != null)
                user.Roles.Add(role);

            if (!user.Transactions.TryGetItem(transaction?.Id, out Transaction transactionEntry) && transaction != null)
                user.Transactions.Add(transaction);

            return user;
        }

        public static Application Map(this Application app, User user, Role role, Transaction transaction)
        {
            app.Author = app.Author ?? user;

            app.Author = app.Author.Map(role, transaction);

            return app;
        }
    }
}
