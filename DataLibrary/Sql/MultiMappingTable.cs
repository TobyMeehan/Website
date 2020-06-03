using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public abstract class MultiMappingTableBase<T> : SqlTable<T>
    {
        private readonly QueryFactory _queryFactory;

        public MultiMappingTableBase(QueryFactory queryFactory) : base(queryFactory)
        {
            _queryFactory = queryFactory;
        }

        protected virtual ExecutableSqlQuery<T> GetSql()
        {
            return _queryFactory.Executable<T>()
                .Select();
        }

        protected abstract IEnumerable<T> Query();
        protected abstract IEnumerable<T> Query(Expression<Predicate<T>> expression);
        protected abstract IEnumerable<T> Query<TForeign>(Expression<Func<T, TForeign, bool>> expression);

        protected abstract Task<IEnumerable<T>> QueryAsync();
        protected abstract Task<IEnumerable<T>> QueryAsync(Expression<Predicate<T>> expression);
        protected abstract Task<IEnumerable<T>> QueryAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression);


        public override IEnumerable<T> Select() => Query();
        public override IEnumerable<T> Select(params string[] columns) => Select();

        public override Task<IEnumerable<T>> SelectAsync() => QueryAsync();
        public override Task<IEnumerable<T>> SelectAsync(params string[] columns) => SelectAsync();

        public override IEnumerable<T> SelectBy(Expression<Predicate<T>> expression) => Query(expression);
        public override IEnumerable<T> SelectBy(Expression<Predicate<T>> expression, params string[] columns) => SelectBy(expression);

        public override Task<IEnumerable<T>> SelectByAsync(Expression<Predicate<T>> expression) => QueryAsync(expression);
        public override Task<IEnumerable<T>> SelectByAsync(Expression<Predicate<T>> expression, params string[] columns) => SelectByAsync(expression);

        public override IEnumerable<T> SelectBy<TForeign>(Expression<Func<T, TForeign, bool>> expression) => Query(expression);
        public override IEnumerable<T> SelectBy<TForeign>(Expression<Func<T, TForeign, bool>> expression, params string[] columns) => SelectBy(expression);

        public override Task<IEnumerable<T>> SelectByAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression) => QueryAsync(expression);
        public override Task<IEnumerable<T>> SelectByAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression, params string[] columns) => SelectByAsync(expression);
    }

    public abstract class MultiMappingTable<T, T1> : MultiMappingTableBase<T>
    {
        public MultiMappingTable(QueryFactory queryFactory) : base(queryFactory)
        {
            
        }

        protected abstract T Map(T value, T1 value1);

        protected override IEnumerable<T> Query() 
            => GetSql().Query<T1>(Map);
        protected override Task<IEnumerable<T>> QueryAsync() 
            => GetSql().QueryAsync<T1>(Map);

        protected override IEnumerable<T> Query(Expression<Predicate<T>> expression) 
            => GetSql().Where(expression).Query<T1>(Map);
        protected override Task<IEnumerable<T>> QueryAsync(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).QueryAsync<T1>(Map);

        protected override IEnumerable<T> Query<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).Query<T1>(Map);
        protected override Task<IEnumerable<T>> QueryAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).QueryAsync<T1>(Map);
    }

    public abstract class MultiMappingTable<T, T1, T2> : MultiMappingTableBase<T>
    {
        public MultiMappingTable(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        protected abstract T Map(T value, T1 value1, T2 value2);

        protected override IEnumerable<T> Query()
            => GetSql().Query<T1, T2>(Map);
        protected override Task<IEnumerable<T>> QueryAsync()
            => GetSql().QueryAsync<T1, T2>(Map);

        protected override IEnumerable<T> Query(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).Query<T1, T2>(Map);
        protected override Task<IEnumerable<T>> QueryAsync(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2>(Map);

        protected override IEnumerable<T> Query<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).Query<T1, T2>(Map);
        protected override Task<IEnumerable<T>> QueryAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2>(Map);
    }

    public abstract class MultiMappingTable<T, T1, T2, T3> : MultiMappingTableBase<T>
    {
        public MultiMappingTable(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        protected abstract T Map(T value, T1 value1, T2 value2, T3 value3);

        protected override IEnumerable<T> Query()
            => GetSql().Query<T1, T2, T3>(Map);
        protected override Task<IEnumerable<T>> QueryAsync()
            => GetSql().QueryAsync<T1, T2, T3>(Map);

        protected override IEnumerable<T> Query(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).Query<T1, T2, T3>(Map);
        protected override Task<IEnumerable<T>> QueryAsync(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2, T3>(Map);

        protected override IEnumerable<T> Query<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).Query<T1, T2, T3>(Map);
        protected override Task<IEnumerable<T>> QueryAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2, T3>(Map);
    }

    public abstract class MultiMappingTable<T, T1, T2, T3, T4> : MultiMappingTableBase<T>
    {
        public MultiMappingTable(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        protected abstract T Map(T value, T1 value1, T2 value2, T3 value3, T4 value4);

        protected override IEnumerable<T> Query()
            => GetSql().Query<T1, T2, T3, T4>(Map);
        protected override Task<IEnumerable<T>> QueryAsync()
            => GetSql().QueryAsync<T1, T2, T3, T4>(Map);

        protected override IEnumerable<T> Query(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).Query<T1, T2, T3, T4>(Map);
        protected override Task<IEnumerable<T>> QueryAsync(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2, T3, T4>(Map);

        protected override IEnumerable<T> Query<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).Query<T1, T2, T3, T4>(Map);
        protected override Task<IEnumerable<T>> QueryAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2, T3, T4>(Map);
    }

    public abstract class MultiMappingTable<T, T1, T2, T3, T4, T5> : MultiMappingTableBase<T>
    {
        public MultiMappingTable(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        protected abstract T Map(T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5);

        protected override IEnumerable<T> Query()
            => GetSql().Query<T1, T2, T3, T4, T5>(Map);
        protected override Task<IEnumerable<T>> QueryAsync()
            => GetSql().QueryAsync<T1, T2, T3, T4, T5>(Map);

        protected override IEnumerable<T> Query(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).Query<T1, T2, T3, T4, T5>(Map);
        protected override Task<IEnumerable<T>> QueryAsync(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2, T3, T4, T5>(Map);

        protected override IEnumerable<T> Query<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).Query<T1, T2, T3, T4, T5>(Map);
        protected override Task<IEnumerable<T>> QueryAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2, T3, T4, T5>(Map);
    }

    public abstract class MultiMappingTable<T, T1, T2, T3, T4, T5, T6> : MultiMappingTableBase<T>
    {
        public MultiMappingTable(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        protected abstract T Map(T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6);

        protected override IEnumerable<T> Query()
            => GetSql().Query<T1, T2, T3, T4, T5, T6>(Map);
        protected override Task<IEnumerable<T>> QueryAsync()
            => GetSql().QueryAsync<T1, T2, T3, T4, T5, T6>(Map);

        protected override IEnumerable<T> Query(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).Query<T1, T2, T3, T4, T5, T6>(Map);
        protected override Task<IEnumerable<T>> QueryAsync(Expression<Predicate<T>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2, T3, T4, T5, T6>(Map);

        protected override IEnumerable<T> Query<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).Query<T1, T2, T3, T4, T5, T6>(Map);
        protected override Task<IEnumerable<T>> QueryAsync<TForeign>(Expression<Func<T, TForeign, bool>> expression)
            => GetSql().Where(expression).QueryAsync<T1, T2, T3, T4, T5, T6>(Map);
    }
}
