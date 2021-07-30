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

        protected virtual Task<IEntityCollection<T>> MapAsync(IEnumerable<T> items) 
        {
            return Task.FromResult<IEntityCollection<T>>(new EntityCollection<T>(items));
        }

        protected async Task<IEntityCollection<T>> SelectAsync(Func<Query, Query> queryFunc = null, int page = 1, int perPage = 50)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                Query query = Query();

                if (queryFunc != null)
                {
                    query = queryFunc.Invoke(query);
                }

                var currentPage = await db.PaginateAsync<T>(query, page, perPage);

                return await MapAsync(currentPage.List);
            }
        }

        protected async Task<T> SelectSingleAsync(Func<Query, Query> queryFunc = null)
        {
            var list = await SelectAsync(queryFunc, perPage: 1);

            return list.FirstOrDefault();
        }
    }
}
