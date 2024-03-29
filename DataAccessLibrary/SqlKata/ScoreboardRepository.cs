﻿using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.Data.SqlKata
{
    public class ScoreboardRepository : RepositoryBase<Objective>, IScoreboardRepository
    {
        private readonly Func<QueryFactory> _queryFactory;
        private readonly IUserRepository _users;

        public ScoreboardRepository(Func<QueryFactory> queryFactory, IUserRepository users) : base(queryFactory)
        {
            _queryFactory = queryFactory;
            _users = users;
        }

        protected override Query Query()
        {
            var scores = new Query("scoreboard").OrderByDesc("Value");

            return base.Query()
                .From("objectives")
                .OrderBy("Name")
                .LeftJoin(scores.As("scores"), j => j.On("scores.ObjectiveId", "objectives.Id"))

                .Select("objectives.{Id, AppId, Name}",
                        "scores.Id AS Scores_Id", "scores.UserId AS Scores_UserId", "scores.Value AS Scores_Value");
        }

        protected override async Task<IEntityCollection<Objective>> MapAsync(IEnumerable<Objective> items)
        {
            foreach (var objective in items)
            {
                foreach (var score in objective.Scores)
                {
                    score.User = await _users.GetByIdAsync(score.UserId);
                }
            }

            return await base.MapAsync(items);
        }



        public async Task<Objective> AddAsync(string appId, string objectiveName)
        {
            string id = Guid.NewGuid().ToToken();

            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("objectives").InsertAsync(new
                {
                    Id = id,
                    AppId = appId,
                    Name = objectiveName
                });
            }

            return await GetByIdAsync(id);
        }



        public async Task<Objective> GetByIdAsync(string id)
        {
            return await SelectSingleAsync(query => query.Where("objectives.Id", id));
        }

        public async Task<IEntityCollection<Objective>> GetByApplicationAsync(string appId)
        {
            return await SelectAsync(query => query.Where("AppId", appId));
        }



        public async Task DeleteAsync(string id)
        {
            using (QueryFactory db = _queryFactory.Invoke())
            {
                await db.Query("objectives").Where("Id", id).DeleteAsync();
            }
        }

        

        public async Task SetScoreAsync(string id, string userId, int value)
        {
            using QueryFactory db = _queryFactory.Invoke();

            var score = await db.Query("scoreboard").Where("ObjectiveId", id).Where("UserId", userId).FirstOrDefaultAsync<Score>();

            if (score == null)
            {
                score.Id = Guid.NewGuid().ToToken();

                await db.Query("scoreboard").InsertAsync(new
                {
                    score.Id,
                    ObjectiveId = id,
                    UserId = userId,
                    Value = 0
                });
            }

            await db.Query("scoreboard").Where("Id", score.Id).UpdateAsync(new
            {
                Value = value
            });
        }
    }
}
