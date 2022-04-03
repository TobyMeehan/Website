using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class RoleRepository : RepositoryBase<IRole, Role, NewRole>, IRoleRepository
    {
        public RoleRepository(QueryFactory queryFactory, IIdGenerator idGenerator) : base (queryFactory, idGenerator, "roles")
        {
        }

        protected override Query Query()
        {
            return base.Query()
                .From("roles")
                .OrderBy("Name");
        }

        public async Task<IRole> GetByNameAsync(string name)
        {
            return await SelectSingleAsync(query => query.Where($"{Table}.Name", name));
        }
    }
}
