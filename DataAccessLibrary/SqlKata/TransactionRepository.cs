using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly IApplicationRepository _applications;

        public TransactionRepository(Func<QueryFactory> queryFactory, IApplicationRepository applications) : base(queryFactory)
        {
            _queryFactory = queryFactory;
            _applications = applications;
        }

        protected override Query Query()
        {
            return base.Query()
                .From("transactions")
                .OrderByDesc("Sent");
        }

        protected override async Task<IEntityCollection<Transaction>> MapAsync(IEnumerable<Transaction> items)
        {
            // Transactions before 2020-08-06 did not record the DateTime they were sent, so are not included
            items = items.ToList().Where(t => t.Sent > new DateTime(2020, 8, 6));

            foreach (var item in items)
            {
                item.Application = await _applications.GetByIdAsync(item.AppId);
            }

            return await base.MapAsync(items);
        }

        public async Task<Transaction> AddAsync(string userId, string appId, string description, int amount)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                string id = await db.Query("transactions").InsertGetIdAsync<string>(new 
                { 
                    Id = Guid.NewGuid().ToToken(),
                    UserId = userId,
                    AppId = appId,
                    Description = description,
                    Amout = amount,
                    Sent = DateTime.Now
                });

                return await GetByIdAsync(id);
            }
        }

        public async Task<IEntityCollection<Transaction>> GetAsync()
        {
            return await SelectAsync();
        }

        public async Task<Transaction> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("Id", id));
        }

        public async Task<IEntityCollection<Transaction>> GetByUserAsync(string userId)
        {
            return await SelectAsync(query => query.Where("UserId", userId));
        }
    }
}
