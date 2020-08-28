using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Sql
{
    public class ConversationTable : MultiMappingTableBase<Conversation>
    {
        public ConversationTable(Func<IDbConnection> connectionFactory) : base(connectionFactory)
        {
        }

        protected override ISqlQuery<Conversation> GetQuery(Dictionary<string, Conversation> dictionary)
        {
            return base.GetQuery(dictionary)
                .LeftJoin<ConversationUser>((c, cu) => c.Id == cu.ConversationId)
                .LeftJoin<ConversationUser, User>((cu, u) => cu.UserId == u.Id);
        }
    }
}
