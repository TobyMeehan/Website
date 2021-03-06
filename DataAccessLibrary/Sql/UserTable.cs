﻿using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class UserTable : MultiMappingTableBase<User>
    {
        public UserTable(Func<IDbConnection> connectionFactory) : base(connectionFactory)
        {
        }

        protected override ISqlQuery<User> GetQuery(Dictionary<string, User> dictionary)
        {
            return new SqlQuery<User>()
                .Select()
                .JoinUsers()
                .Map<Role>((user, role) =>
                {
                    if (!dictionary.TryGetValue(user.Id, out User entry))
                    {
                        entry = user;
                        entry.Roles = new EntityCollection<Role>();

                        dictionary.Add(entry.Id, entry);
                    }

                    return entry.Map(role);
                });
        }
    }
}
