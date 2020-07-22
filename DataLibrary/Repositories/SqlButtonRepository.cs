using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlButtonRepository : SqlRepository<ButtonPress>, IButtonRepository
    {
        private readonly ISqlTable<ButtonPress> _table;

        public SqlButtonRepository(ISqlTable<ButtonPress> table) : base(table)
        {
            _table = table;
        }

        public Task AddAsync(string userId, TimeSpan buttonTimeSpan)
        {
            string id = Guid.NewGuid().ToString();

            return _table.InsertAsync(new
            {
                Id = id,
                UserId = userId,
                TimePressed = DateTime.Now,
                ButtonSeconds = buttonTimeSpan.Seconds
            });
        }

        public async Task<IList<ButtonPress>> GetByUserAsync(string userId)
        {
            return (await _table.SelectByAsync(b => b.UserId == userId)).ToList();
        }
    }
}
