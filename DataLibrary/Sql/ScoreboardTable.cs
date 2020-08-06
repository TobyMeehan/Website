using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class ScoreboardTable : MultiMappingTableBase<Objective>
    {
        public ScoreboardTable(Func<IDbConnection> connectionFactory) : base(connectionFactory)
        {

        }

        protected override ISqlQuery<Objective> GetQuery(Dictionary<string, Objective> dictionary)
        {
            var query = base.GetQuery(dictionary)
                .LeftJoin<Score>((o, s) => s.ObjectiveId == o.Id)
                .Map<Score>((objective, score) =>
                {
                    if (!dictionary.TryGetValue(objective.Id, out Objective entry))
                    {
                        entry = objective;
                        entry.Scores = new EntityCollection<Score>();

                        dictionary.Add(entry.Id, entry);
                    }

                    if (score != null && !entry.Scores.TryGetItem(score?.Id, out Score scoreEntry))
                    {
                        entry.Scores.Add(score);
                    }

                    return entry;
                });

            string fuckyou = query.ToSql();

            return query;
        }
    }
}
