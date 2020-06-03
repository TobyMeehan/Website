using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Extensions
{
    public static class SqlQueryExtensions
    {
        public static TQuery JoinUsers<T, TQuery>(this SqlQueryBase<T, TQuery> query) where TQuery : SqlQueryBase<T, TQuery>
        {
            return query
                .LeftJoin<User, UserRole>((u, ur) => u.Id == ur.UserId)
                .LeftJoin<UserRole, Role>((ur, r) => ur.RoleId == r.Id)
                .LeftJoin<User, Transaction>((u, t) => u.Id == t.UserId);

        }

        public static TQuery JoinApplications<T, TQuery>(this SqlQueryBase<T, TQuery> query) where TQuery : SqlQueryBase<T, TQuery>
        {
            return query
                .InnerJoin<Application, User>((a, u) => a.UserId == u.Id)
                .JoinUsers();
        }

        public static TQuery JoinConnections<T, TQuery>(this SqlQueryBase<T, TQuery> query) where TQuery : SqlQueryBase<T, TQuery>
        {
            return query
                .InnerJoin<Connection, User>((c, u) => c.UserId == u.Id)
                .InnerJoin<Connection, Application>((c, a) => c.AppId == a.Id)
                .JoinApplications();
        }

        public static TQuery JoinDownloads<T, TQuery>(this SqlQueryBase<T, TQuery> query) where TQuery : SqlQueryBase<T, TQuery>
        {
            return query
                .InnerJoin<Download, DownloadAuthor>((d, da) => d.Id == da.DownloadId)
                .InnerJoin<DownloadAuthor, User>((da, u) => da.UserId == u.Id)
                .JoinUsers()
                .LeftJoin<Download, DownloadFile>((d, df) => d.Id == df.DownloadId);
        }
    }
}
