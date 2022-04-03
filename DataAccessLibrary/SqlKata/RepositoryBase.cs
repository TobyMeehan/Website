using Slapper;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Extensions;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class RepositoryBase
    {
        protected readonly QueryFactory QueryFactory;

        public RepositoryBase(QueryFactory queryFactory)
        {
            QueryFactory = queryFactory;
        }

        protected virtual Query Query()
        {
            return new Query();
        }

        private Query Query(int page, int perPage)
        {
            var query = Query();
            var clause = query.Clauses.FirstOrDefault(c => c is FromClause) as FromClause;

            if (clause == null)
            {
                return query.Limit(200);
            }

            return query.From(new Query(clause.Table), clause.Alias).ForPage(page, perPage);
        }

        protected virtual Task<IReadOnlyList<T1>> MapAsync<T1, T2>(IEnumerable<T2> items)
        {
            return Task.FromResult(Map<T1, T2>(items));
        }

        private IReadOnlyList<T1> Map<T1, T2>(IEnumerable<T2> items)
        {
            return items.Cast<T1>().ToList().AsReadOnly();
        }

        protected async Task<IReadOnlyList<T>> SelectAsync<T>(Func<Query, Query> queryFunc = null, int page = 1,
            int perPage = 200)
        {
            var query = Query(page, perPage);

            if (queryFunc != null)
            {
                query = queryFunc.Invoke(query);
            }

            var result = await QueryFactory.GetAsync(query);
            
            return AutoMapper.MapDynamic<T>(result).ToList().AsReadOnly();
        }

        protected Task<T> SelectSingleAsync<T>(Func<Query, Query> queryFunc = null)
        {
            return SelectSingleAsync<T, T>(queryFunc);
        }

        protected async Task<IReadOnlyList<T1>> SelectAsync<T1, T2>(Func<Query, Query> queryFunc = null, int page = 1, int perPage = 200)
        {
            var list = await SelectAsync<T2>(queryFunc, page, perPage);

            return await MapAsync<T1, T2>(list);
        }

        protected async Task<T1> SelectSingleAsync<T1, T2>(Func<Query, Query> queryFunc = null)
        {
            var list = await SelectAsync<T1, T2>(queryFunc, perPage: 1);

            return list.FirstOrDefault();
        }
    }
    
    public class RepositoryBase<TOut, TIn> : RepositoryBase where TIn : TOut
    {
        public RepositoryBase(QueryFactory queryFactory) : base(queryFactory)
        {
        }

        protected virtual Task<IReadOnlyList<TOut>> MapAsync(IEnumerable<TIn> items)
        {
            return base.MapAsync<TOut, TIn>(items);
        }

        protected Task<IReadOnlyList<TOut>> SelectAsync(Func<Query, Query> queryFunc = null, int page = 1,
            int perPage = 200)
        {
            return base.SelectAsync<TOut, TIn>(queryFunc, page, perPage);
        }

        protected Task<TOut> SelectSingleAsync(Func<Query, Query> queryFunc = null)
        {
            return base.SelectSingleAsync<TOut, TIn>(queryFunc);
        }
    }

    public class RepositoryBase<TOut, TIn, TNew> : RepositoryBase<TOut, TIn> where TIn : TOut where TNew : new()
    {
        private readonly IIdGenerator _idGenerator;
        protected readonly string Table;

        public RepositoryBase(QueryFactory queryFactory, IIdGenerator idGenerator, string table) : base(queryFactory)
        {
            _idGenerator = idGenerator;
            Table = table;
        }

        protected virtual Id<TOut> GenerateId()
        {
            return _idGenerator.GenerateId<TOut>();
        }

        public virtual Task<IReadOnlyList<TOut>> GetAsync()
        {
            return SelectAsync();
        }

        public virtual Task<TOut> GetByIdAsync(Id<TOut> id)
        {
            return SelectSingleAsync(query => query.Where($"{Table}.Id", id.Value));
        }

        public async Task<TOut> AddAsync(Action<TNew> configure)
        {
            return await AddAsync(configure, null);
        }
        
        protected virtual async Task<TOut> AddAsync(Action<TNew> configure, Id<TOut>? id)
        {
            var obj = configure.Apply(new TNew());

            id ??= _idGenerator.GenerateId<TOut>();
            
            await QueryFactory.Query(Table).InsertAsync(new {Id = id.Value.Value});
            
            await QueryFactory.Query(Table).Where("Id", id).UpdateAsync(obj);

            return await GetByIdAsync(id.Value);
        }
        
        public virtual async Task DeleteAsync(Id<TOut> id)
        {
            await QueryFactory.Query(Table).Where("Id", id.Value).DeleteAsync();
        }
    }
    
    public class RepositoryBase<TOut, TIn, TNew, TEdit> : RepositoryBase<TOut, TIn, TNew> where TIn : TOut where TNew : new() where TEdit : new()
    {
        public RepositoryBase(QueryFactory queryFactory, IIdGenerator idGenerator, string table) : base(queryFactory, idGenerator, table)
        {
        }

        public virtual async Task<TOut> UpdateAsync(Id<TOut> id, Action<TEdit> configure)
        {
            var record = await SelectSingleAsync<TEdit>(query => query.Where($"{Table}.Id", id.Value));
            configure(record);
            
            await QueryFactory.Query(Table).Where("Id", id.Value).UpdateAsync(record);

            return await GetByIdAsync(id);
        }
    }
}
