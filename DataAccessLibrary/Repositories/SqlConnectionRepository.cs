﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlConnectionRepository : SqlRepository<Connection>, IConnectionRepository
    {
        private readonly ISqlTable<Connection> _table;

        public SqlConnectionRepository(ISqlTable<Connection> table) : base(table)
        {
            _table = table;
        }

        public async Task<IList<Connection>> GetByApplicationAsync(string appId)
        {
            return (await _table.SelectByAsync(c => c.AppId == appId)).ToList();
        }

        public async Task<Connection> GetOrCreateAsync(string userId, string appId)
        {
            Connection connection = (await _table.SelectByAsync(c => c.UserId == userId && c.AppId == appId)).SingleOrDefault();

            if (connection == null)
            {
                string id = Guid.NewGuid().ToString();

                await _table.InsertAsync(new
                {
                    Id = id,
                    UserId = userId,
                    AppId = appId
                });
            }

            return connection ?? (await _table.SelectByAsync(c => c.UserId == userId && c.AppId == appId)).SingleOrDefault();
        }

        public async Task<IList<Connection>> GetByUserAsync(string userId)
        {
            return (await _table.SelectByAsync(c => c.UserId == userId)).ToList();
        }
    }
}
