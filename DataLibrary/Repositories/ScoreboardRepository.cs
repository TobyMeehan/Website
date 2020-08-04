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
    public class ScoreboardRepository : SqlRepository<Objective>, IScoreboardRepository
    {
        private readonly ISqlTable<Objective> _table;
        private readonly ISqlTable<Score> _scoreTable;
        private readonly IUserRepository _users;

        public ScoreboardRepository(ISqlTable<Objective> table, ISqlTable<Score> scoreTable, IUserRepository users) : base(table)
        {
            _table = table;
            _scoreTable = scoreTable;
            _users = users;
        }

        protected override async Task<IEnumerable<Objective>> FormatAsync(IEnumerable<Objective> values)
        {
            foreach (var objective in values)
            {
                foreach (var score in objective.Scores)
                {
                    score.User = await _users.GetByIdAsync(score.UserId);
                }
            }

            return values;
        }

        public async Task<IList<Objective>> GetByApplicationAsync(string appId)
        {
            return (await SelectAsync(x => x.AppId == appId)).ToList();
        }

        public async Task<Objective> AddAsync(string appId, string objectiveName)
        {
            string id = Guid.NewGuid().ToToken();

            await _table.InsertAsync(new
            {
                Id = id,
                AppId = appId,
                Name = objectiveName
            });

            return await GetByIdAsync(id);
        }

        public async Task SetScoreAsync(string id, string userId, int value)
        {
            var score = (await _scoreTable.SelectByAsync(x => x.ObjectiveId == id && x.UserId == userId)).SingleOrDefault();

            if (score == null)
            {
                await _scoreTable.InsertAsync(new
                {
                    ObjectiveId = id,
                    UserId = userId,
                    Value = 0
                });
            }

            await _scoreTable.UpdateAsync(x => x.ObjectiveId == id && x.UserId == userId, new
            {
                Value = value
            });
        }
    }
}
