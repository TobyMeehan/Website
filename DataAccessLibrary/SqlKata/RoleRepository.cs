using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        private readonly Func<QueryFactory> _queryFactory;

        public RoleRepository(Func<QueryFactory> queryFactory) : base (queryFactory)
        {
            _queryFactory = queryFactory;
        }

        protected override Query Query()
        {
            return base.Query()
                .From("roles");
        }


        public async Task<Role> AddAsync(string name)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                string id = await db.Query("roles").InsertGetIdAsync<string>(new
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name
                });

                return await GetByIdAsync(id);
            }
        }



        public async Task<IEntityCollection<Role>> GetAsync()
        {
            return await SelectAsync();
        }

        public async Task<Role> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("Id", id));
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            return await SelectSingleAsync(query => query.Where("Name", name));
        }




        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("roles").Where("Id", id).DeleteAsync();
            }
        }
    }
}
