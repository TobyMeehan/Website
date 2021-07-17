using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Data.Sql;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Configuration
{
    public class DataAccessLibraryBuilder
    {
        public DataAccessLibraryBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; set; }

        public DataAccessLibraryBuilder AddSqlDatabase(Func<IDbConnection> connectionFactory)
        {
            Services.AddSingleton(connectionFactory);

            var compiler = new MySqlCompiler();
            Services.AddSingleton<Func<QueryFactory>>(() => new QueryFactory(connectionFactory.Invoke(), compiler));

            Services.AddTransient<ISqlTable<User>, UserTable>();
            Services.AddTransient<ISqlTable<Application>, ApplicationTable>();
            Services.AddTransient<ISqlTable<Connection>, ConnectionTable>();
            Services.AddTransient<ISqlTable<Objective>, ScoreboardTable>();
            Services.AddTransient<ISqlTable<Download>, DownloadTable>();
            Services.AddTransient<ISqlTable<Conversation>, ConversationTable>();
            Services.AddTransient(typeof(ISqlTable<>), typeof(SqlTable<>));


            Services.AddTransient<IUserRepository, SqlUserRepository>();
            Services.AddTransient<IRoleRepository, SqlRoleRepository>();
            Services.AddTransient<ITransactionRepository, TransactionRepository>();

            Services.AddTransient<IDownloadRepository, SqlDownloadRepository>();
            Services.AddTransient<IDownloadFileRepository, DownloadFileRepository>();

            Services.AddTransient<IApplicationRepository, SqlApplicationRepository>();
            Services.AddTransient<IConnectionRepository, SqlConnectionRepository>();
            Services.AddTransient<IOAuthSessionRepository, SqlOAuthSessionRepository>();

            Services.AddTransient<IScoreboardRepository, ScoreboardRepository>();

            Services.AddTransient<IMessageRepository, SqlMessageRepository>();
            Services.AddTransient<ICommentRepository, SqlCommentRepository>();

            Services.AddTransient<IButtonRepository, SqlButtonRepository>();

            return this;
        }

        public DataAccessLibraryBuilder AddBCryptPasswordHash()
        {
            Services.AddSingleton<IPasswordHash, BCryptPasswordHash>();

            return this;
        }

        public DataAccessLibraryBuilder AddGoogleCloudStorage(GoogleCredential storageCredential, Action<CloudStorageOptions> configureOptions)
        {
            Services.AddTransient<ICloudStorage, GoogleCloudStorage>();
            Services.AddSingleton(storageCredential);

            Services.Configure(configureOptions);

            return this;
        }

        public DataAccessLibraryBuilder AddDefaultCloudStorage()
        {
            Services.AddTransient<ICloudStorage, DefaultCloudStorage>();

            return this;
        }

        public DataAccessLibraryBuilder AddDefaultTokenProvider()
        {
            Services.AddTransient<ITokenProvider, DefaultTokenProvider>();

            return this;
        }
    }
}
