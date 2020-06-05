using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Extensions
{
    public static class SqlQueryExtensions
    {
        public static ISqlQuery<T> JoinUsers<T>(this ISqlQuery<T> query)
        {
            return query
                .LeftJoin<User, UserRole>((u, ur) => u.Id == ur.UserId)
                .LeftJoin<UserRole, Role>((ur, r) => ur.RoleId == r.Id)
                .LeftJoin<User, Transaction>((u, t) => u.Id == t.UserId);

        }

        public static ISqlQuery<T> JoinApplications<T>(this ISqlQuery<T> query)
        {
            return query
                .InnerJoin<Application, User>((a, u) => a.UserId == u.Id)
                .JoinUsers();
        }

        public static ISqlQuery<T> JoinConnections<T>(this ISqlQuery<T> query)
        {
            return query
                .InnerJoin<Connection, User>((c, u) => c.UserId == u.Id)
                .InnerJoin<Connection, Application>((c, a) => c.AppId == a.Id)
                .JoinApplications();
        }

        public static ISqlQuery<T> JoinDownloads<T>(this ISqlQuery<T> query)
        {
            return query
                .InnerJoin<Download, DownloadAuthor>((d, da) => d.Id == da.DownloadId)
                .InnerJoin<DownloadAuthor, User>((da, u) => da.UserId == u.Id)
                .JoinUsers()
                .LeftJoin<Download, DownloadFile>((d, df) => d.Id == df.DownloadId);
        }
    }
}
