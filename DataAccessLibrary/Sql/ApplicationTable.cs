using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class ApplicationTable : MultiMappingTableBase<Application>
    {
        public ApplicationTable(Func<IDbConnection> connectionFactory) : base(connectionFactory)
        {

        }

        protected override ISqlQuery<Application> GetQuery(Dictionary<string, Application> dictionary)
        {
            return base.GetQuery(dictionary)
                .JoinApplications()
                .Map<User, Role>((app, user, role) =>
                {
                    if (!dictionary.TryGetValue(app.Id, out Application entry))
                    {
                        entry = app;
                    }

                    return entry.Map(user, role);
                });
        }
    }
}
