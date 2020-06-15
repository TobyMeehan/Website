using Microsoft.Extensions.DependencyInjection;
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

namespace TobyMeehan.Com.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataAccessLibrary(this IServiceCollection services, Action<DataAccessLibraryOptions> configureOptions)
        {
            DataAccessLibraryOptions opt = new DataAccessLibraryOptions();
            configureOptions(opt);

            services.AddSingleton(opt.ConnectionFactory);

            services.AddTransient<ISqlTable<User>, UserTable>();
            services.AddTransient<ISqlTable<Application>, ApplicationTable>();
            services.AddTransient<ISqlTable<Connection>, ConnectionTable>();
            services.AddTransient<ISqlTable<Download>, DownloadTable>();
            services.AddTransient(typeof(ISqlTable<>), typeof(SqlTable<>));


            services.AddTransient<ICloudStorage, GoogleCloudStorage>();
            services.AddSingleton(opt.StorageCredential);


            services.AddTransient<IUserRepository, SqlUserRepository>();
            services.AddTransient<IRoleRepository, SqlRoleRepository>();

            services.AddTransient<IDownloadRepository, SqlDownloadRepository>();
            services.AddTransient<IDownloadFileRepository, DownloadFileRepository>();

            services.AddTransient<IApplicationRepository, SqlApplicationRepository>();
            services.AddTransient<IConnectionRepository, SqlConnectionRepository>();

            services.AddSingleton<IPasswordHash, BCryptPasswordHash>();


            services.Configure(configureOptions);
        }
    }
}
