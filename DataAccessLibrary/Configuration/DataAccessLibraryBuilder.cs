using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Data.Configuration
{
    public class DataAccessLibraryBuilder
    {
        public DataAccessLibraryBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; set; }

        public DataAccessLibraryBuilder AddSqlDatabase<TCompiler>(Func<IDbConnection> connectionFactory) where TCompiler : Compiler
        {
            Services.AddTransient<Compiler, TCompiler>();
            Services.AddSingleton<Func<IDbConnection>>(connectionFactory);

            Services.AddTransient<QueryFactory>(services =>
            {
                var connection = services.GetService<Func<IDbConnection>>().Invoke();
                var compiler = services.GetService<Compiler>();

                return new QueryFactory(connection, compiler);
            });

            Services.AddTransient<IUserRepository, SqlKata.UserRepository>();
            Services.AddTransient<IRoleRepository, SqlKata.RoleRepository>();
            Services.AddTransient<ITransactionRepository, SqlKata.TransactionRepository>();

            Services.AddTransient<IDownloadRepository, SqlKata.DownloadRepository>();
            Services.AddTransient<IDownloadFileRepository, SqlKata.DownloadFileRepository>();

            Services.AddTransient<IApplicationRepository, SqlKata.ApplicationRepository>();
            Services.AddTransient<IConnectionRepository, SqlKata.ConnectionRepository>();
            Services.AddTransient<IOAuthSessionRepository, SqlKata.OAuthSessionRepository>();

            Services.AddTransient<IScoreboardRepository, SqlKata.ScoreboardRepository>();

            Services.AddTransient<ICommentRepository, SqlKata.CommentRepository>();
            
            Slapper.AutoMapper.Configuration.TypeConverters.Add(new IdConverter());
            
            Slapper.AutoMapper.Configuration.TypeActivators.Add(new SqlKata.ApplicationActivator());

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

        public DataAccessLibraryBuilder AddSymmetricTokenProvider(Action<SymmetricTokenOptions> configureOptions)
        {
            Services.AddSingleton<ITokenProvider, SymmetricTokenProvider>();

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
