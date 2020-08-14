using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class CommentTable : MultiMappingTableBase<Comment>
    {
        public CommentTable(Func<IDbConnection> connectionFactory) : base (connectionFactory)
        {

        }

        protected override ISqlQuery<Comment> GetQuery(Dictionary<string, Comment> dictionary)
        {
            return base.GetQuery(dictionary)
                .InnerJoin<Comment, User>((c, u) => c.UserId == u.Id)
                .JoinUsers()
                .Map<User, Role>((comment, user, role) =>
                {
                    if (!dictionary.TryGetValue(comment.Id, out Comment entry))
                    {
                        entry = comment;
                        dictionary.Add(entry.Id, entry);
                    }

                    entry.User = user.Map(role);

                    return entry;
                });
        }
    }
}
