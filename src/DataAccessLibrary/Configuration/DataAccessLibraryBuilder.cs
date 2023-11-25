using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SqlKata.Compilers;
using TobyMeehan.Com.Data.Authorization;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.DataAccess.Configuration;
using TobyMeehan.Com.Data.Domain.Applications;
using TobyMeehan.Com.Data.Domain.Applications.Repositories;
using TobyMeehan.Com.Data.Domain.Authorizations;
using TobyMeehan.Com.Data.Domain.Authorizations.Repositories;
using TobyMeehan.Com.Data.Domain.Avatars;
using TobyMeehan.Com.Data.Domain.Avatars.Repositories;
using TobyMeehan.Com.Data.Domain.Scopes;
using TobyMeehan.Com.Data.Domain.Scopes.Repositories;
using TobyMeehan.Com.Data.Domain.Tokens;
using TobyMeehan.Com.Data.Domain.Tokens.Repositories;
using TobyMeehan.Com.Data.Domain.Transactions;
using TobyMeehan.Com.Data.Domain.Transactions.Repositories;
using TobyMeehan.Com.Data.Domain.UserRoles;
using TobyMeehan.Com.Data.Domain.UserRoles.Repositories;
using TobyMeehan.Com.Data.Domain.Users;
using TobyMeehan.Com.Data.Domain.Users.Repositories;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Data.Security.BCrypt;
using TobyMeehan.Com.Data.Storage;
using TobyMeehan.Com.Data.Storage.Configuration;
using TobyMeehan.Com.Data.Storage.Google;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Configuration;

public class DataAccessLibraryBuilder
{
    public DataAccessLibraryBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public DataAccessLibraryBuilder(IServiceCollection services, IConfiguration configuration)
    {
        Services = services;
        
        Configure(configuration);
    }
    
    public IServiceCollection Services { get; }

    private void Configure(IConfiguration configuration)
    {
        if (configuration.GetSection("Postgres") is { } postgresConfig && 
            postgresConfig.Get<PostgresOptions>() is { } postgresOptions)
        {
            Services.Configure<PostgresOptions>(postgresConfig);
            this.AddPostgresDatabase(postgresOptions);
        }

        if (configuration.GetSection("Storage") is { } storageConfig &&
            storageConfig.Get<StorageOptions>() is { } storageOptions)
        {
            Services.Configure<StorageOptions>(storageConfig);

            if (storageConfig.GetSection("Google") is { } googleStorageConfig &&
                googleStorageConfig.Get<GoogleStorageOptions>() is { } googleStorageOptions)
            {
                Services.Configure<GoogleStorageOptions>(googleStorageConfig);
                this.AddGoogleCloudStorage(storageOptions, googleStorageOptions);
            }
        }

        if (configuration.GetSection("Domain:Users") is { } usersConfig &&
            usersConfig.Get<UserOptions>() is { } userOptions)
        {
            Services.Configure<UserOptions>(usersConfig);
        }
    }

    public DataAccessLibraryBuilder AddDatabase<TDbConnectionFactory, TCompiler, TSqlDataAccess>() 
        where TDbConnectionFactory : class, IDbConnectionFactory 
        where TCompiler : Compiler
        where TSqlDataAccess : class, ISqlDataAccess
    {
        Services.AddSingleton<IDbConnectionFactory, TDbConnectionFactory>();
        Services.AddSingleton<Compiler, TCompiler>();

        Services.AddTransient<ISqlDataAccess, TSqlDataAccess>();

        return this;
    }
    
    public DataAccessLibraryBuilder AddCloudStorage<T>(StorageOptions options) 
        where T : class, IStorageService
    {
        Services.AddTransient<IStorageService, T>();

        Services.AddTransient<IAvatarService, StorageEnabledAvatarService>();
        
        return this;
    }
    
    public DataAccessLibraryBuilder AddSqlKataRepositories()
    {
        Services.AddTransient<IApplicationRepository, ApplicationRepository>();
        Services.AddTransient<IAuthorizationRepository, AuthorizationRepository>();
        Services.AddTransient<IAvatarRepository, AvatarRepository>();
        Services.AddTransient<IScopeRepository, ScopeRepository>();
        Services.AddTransient<ITokenRepository, TokenRepository>();
        Services.AddTransient<ITransactionRepository, TransactionRepository>();
        Services.AddTransient<IUserRepository, UserRepository>();
        Services.AddTransient<IUserRoleRepository, UserRoleRepository>();
        
        return this;
    }

    public DataAccessLibraryBuilder AddEntityServices()
    {
        Services.AddTransient<IApplicationService, ApplicationService>();
        Services.AddTransient<IAuthorizationService, AuthorizationService>();
        Services.AddTransient<IAvatarService, AvatarService>();
        Services.AddTransient<IScopeService, ScopeService>();
        Services.AddTransient<ITokenService, TokenService>();
        Services.AddTransient<ITransactionService, TransactionService>();
        Services.AddTransient<IUserService, UserService>();
        Services.AddTransient<IUserRoleService, UserRoleService>();
        
        Services.AddSingleton(typeof(ICacheService<,>), typeof(MemoryCacheService<,>));
        
        return this;
    }

    public DataAccessLibraryBuilder AddScopeValidation()
    {
        Services.AddTransient<IScopeValidator, UserRoleScopeValidator>();

        return this;
    }

    public DataAccessLibraryBuilder AddBCryptPasswordHash()
    {
        Services.AddTransient<IPasswordService, BCryptPasswordService>();

        return this;
    }

    public DataAccessLibraryBuilder AddBase64IdGeneration()
    {
        Services.AddTransient<IIdService, Base64GuidIdService>();

        return this;
    }

    public DataAccessLibraryBuilder AddRngSecretService()
    {
        Services.AddSingleton<ISecretService, RandomNumberGeneratorSecretService>();

        return this;
    }
}