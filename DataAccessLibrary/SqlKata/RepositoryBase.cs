using Slapper;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class RepositoryBase<T> where T : EntityBase
    {
        private readonly Func<QueryFactory> _queryFactory;

        public RepositoryBase(Func<QueryFactory> queryFactory)
        {
            _queryFactory = queryFactory;
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

            return query.From(new Query(clause.Table).ForPage(page, perPage).As(clause.Alias));
        }

        protected virtual Task<IEntityCollection<T>> MapAsync(IEnumerable<T> items) 
        {
            return Task.FromResult<IEntityCollection<T>>(new EntityCollection<T>(items));
        }

        protected async Task<IEntityCollection<T>> SelectAsync(Func<Query, Query> queryFunc = null, int page = 1, int perPage = 200)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                Query query = Query(page, perPage);

                if (queryFunc != null)
                {
                    query = queryFunc.Invoke(query);
                }

                var result = await db.GetAsync(query);
                var list = AutoMapper.MapDynamic<T>(result);

                return await MapAsync(list);
            }
        }

        protected async Task<T> SelectSingleAsync(Func<Query, Query> queryFunc = null)
        {
            var list = await SelectAsync(queryFunc);

            return list.FirstOrDefault();
        }
    }
}
