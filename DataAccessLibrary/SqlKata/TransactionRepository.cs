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

        public TransactionRepository(Func<QueryFactory> queryFactory) : base(queryFactory)
        {
            _queryFactory = queryFactory;
        }

        protected override Query Query()
        {
            var apps = new Query("applications");

            return base.Query()
                .From("transactions")
                .OrderByDesc("Sent")
                .LeftJoin(apps.As("apps"), j => j.On("apps.Id", "transactions.AppId"))

                .Select("transactions.{Id, UserId, AppId, Description, Amount, Sent}",
                        "apps.Id AS Application_Id", "apps.UserId AS Application_UserId", "apps.Name AS Application_Name", "apps.Description AS Application_Description", "apps.IconUrl AS Application_IconUrl", "apps.RedirectUri AS Application_RedirectUri");
        }

        protected override async Task<IEntityCollection<Transaction>> MapAsync(IEnumerable<Transaction> items)
        {
            // Transactions before 2020-08-06 did not record the DateTime they were sent, so are not included
            items = items.ToList().Where(t => t.Sent > new DateTime(2020, 8, 6));

            return await base.MapAsync(items);
        }

        public async Task<Transaction> AddAsync(string userId, string appId, string description, int amount)
        {
            string id = Guid.NewGuid().ToToken();

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("transactions").InsertAsync(new 
                { 
                    Id = id,
                    UserId = userId,
                    AppId = appId,
                    Description = description,
                    Amout = amount,
                    Sent = DateTime.Now
                });
            }

            return await GetByIdAsync(id);
        }

        public async Task<IEntityCollection<Transaction>> GetAsync()
        {
            return await SelectAsync();
        }

        public async Task<Transaction> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("transactions.Id", id));
        }

        public async Task<IEntityCollection<Transaction>> GetByUserAsync(string userId, int page = 1, int perPage = 15)
        {
            return await SelectAsync(query => query.Where("transactions.UserId", userId), page, perPage);
        }
        public async Task<int> TotalPagesForUserAsync(string userId, int perPage = 15)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                decimal count = (decimal)(await db.Query("transactions").SelectRaw("COUNT(*) AS Count").Where("UserId", userId).FirstAsync()).Count;

                return (int)Math.Ceiling(perPage / count);
            }
        }
    }
}
