using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class TransactionRepository : SqlRepository<Transaction>, ITransactionRepository
    {
        private readonly ISqlTable<Transaction> _table;
        private readonly IApplicationRepository _applications;

        public TransactionRepository(ISqlTable<Transaction> table, IApplicationRepository applications) : base(table)
        {
            _table = table;
            _applications = applications;
        }

        protected override async Task<IEnumerable<Transaction>> FormatAsync(IEnumerable<Transaction> values)
        {
            foreach (var transaction in values)
            {
                transaction.Application = await _applications.GetByIdAsync(transaction.AppId);
            }

            return await base.FormatAsync(values);
        }

        public async Task<Transaction> AddAsync(string userId, string appId, string description, int amount)
        {
            string id = Guid.NewGuid().ToToken();

            await _table.InsertAsync(new
            {
                Id = id,
                UserId = userId,
                AppId = appId,
                Description = description,
                Amount = amount,
                Sent = DateTime.Now
            });

            return await GetByIdAsync(id);
        }

        public async Task<IList<Transaction>> GetByUserAsync(string userId)
        {
            return (await SelectAsync(t => t.UserId == userId)).ToList();
        }
    }
}
