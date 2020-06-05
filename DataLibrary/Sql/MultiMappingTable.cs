using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public abstract class MultiMappingTableBase<T> : SqlTable<T> where T : EntityBase
    {
        private readonly Func<IDbConnection> _connectionFactory;

        public MultiMappingTableBase(Func<IDbConnection> connectionFactory) : base(connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private ISqlQuery<T> GetQuery() => GetQuery(new Dictionary<string, T>());

        protected virtual ISqlQuery<T> GetQuery(Dictionary<string, T> dictionary)
        {
            return new SqlQuery<T>()
                .Select();
        }

        private IEnumerable<T> Query()
        {
            using (IDbConnection connection = _connectionFactory.Invoke())
            {
                return connection.Query(GetQuery());
            }
        }

        private IEnumerable<T> Query(Expression<Predicate<T>> expression)
        {
            using (IDbConnection connection = _connectionFactory.Invoke())
            {
                return connection.Query(GetQuery().Where(expression));
            }
        }

        private IEnumerable<T> Query<TForeign>(Expression<Func<T, TForeign, bool>> expression)
        {
            using (IDbConnection connection = _connectionFactory.Invoke())
            {
                return connection.Query(GetQuery().Where(expression));
            }
        }

        private Task<IEnumerable<T>> QueryAsync()
        {
            using (IDbConnection connection = _connectionFactory.Invoke())
            {
                return connection.QueryAsync(GetQuery());
            }
        }
        private Task<IEnumerable<T>> QueryAsync(Expression<Predicate<T>> expression)
        {
            using (IDbConnection connection = _connectionFactory.Invoke())
            {
                return connection.QueryAsync(GetQuery().Where(expression));
            }
        }
        private Task<IEnumerable<T>> QueryAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression)
        {
            using (IDbConnection connection = _connectionFactory.Invoke())
            {
                return connection.QueryAsync(GetQuery().Where(expression));
            }
        }


        public override IEnumerable<T> Select() => Query().Distinct();
        public override IEnumerable<T> Select(params string[] columns) => Select();

        public override async Task<IEnumerable<T>> SelectAsync() => (await QueryAsync()).Distinct();
        public override Task<IEnumerable<T>> SelectAsync(params string[] columns) => SelectAsync();

        public override IEnumerable<T> SelectBy(Expression<Predicate<T>> expression) => Query(expression).Distinct();
        public override IEnumerable<T> SelectBy(Expression<Predicate<T>> expression, params string[] columns) => SelectBy(expression);

        public override async Task<IEnumerable<T>> SelectByAsync(Expression<Predicate<T>> expression)
            => (await QueryAsync(expression)).Distinct();
        public override Task<IEnumerable<T>> SelectByAsync(Expression<Predicate<T>> expression, params string[] columns) => SelectByAsync(expression);

        public override IEnumerable<T> SelectBy<TForeign>(Expression<Func<T, TForeign, bool>> expression) => Query(expression).Distinct();
        public override IEnumerable<T> SelectBy<TForeign>(Expression<Func<T, TForeign, bool>> expression, params string[] columns) => SelectBy(expression);

        public override async Task<IEnumerable<T>> SelectByAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => (await QueryAsync(expression)).Distinct();
        public override Task<IEnumerable<T>> SelectByAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression, params string[] columns) => SelectByAsync(expression);
    }
}
