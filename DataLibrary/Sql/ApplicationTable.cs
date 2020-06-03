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
    public class ApplicationTable : MultiMappingTable<Application, User, Role, Transaction>
    {
        private readonly QueryFactory _factory;

        public ApplicationTable(QueryFactory factory) : base(factory)
        {
            _factory = factory;
        }

        protected override ExecutableSqlQuery<Application> GetSql()
        {
            return _factory.Executable<Application>()
                .Select()
                .JoinApplications();
        }

        protected override Application Map(Application app, User user, Role role, Transaction transaction)
        {
            return app.Map(user, role, transaction);
        }
    }
}
