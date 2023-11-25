using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using TobyMeehan.Com.Data.Security.Configuration;
using TobyMeehan.Com.Data.Security.DataProtection;
using TobyMeehan.Com.Data.Security.Passwords;
using TobyMeehan.Com.Data.Storage;
using TobyMeehan.Com.Data.Storage.Configuration;
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
        foreach (var section in configuration.GetChildren())
            switch (section.Key)
            {
                case "Postgres" when section.Get<PostgresOptions>() is { } postgresOptions:
                    Services.Configure<PostgresOptions>(section);
                    this.AddPostgresDatabase(postgresOptions);
                    break;
                
                case "Storage" when section.Get<StorageOptions>() is { } storageOptions:
                    Services.Configure<StorageOptions>(section);
                    
                    if (section.GetSection("Google") is { } googleStorageConfig &&
                        googleStorageConfig.Get<GoogleStorageOptions>() is { } googleStorageOptions)
                    {
                        Services.Configure<GoogleStorageOptions>(googleStorageConfig);
                        this.AddGoogleCloudStorage(googleStorageOptions);
                    }
                    
                    break;
                
                case "Security" when section.Get<SecurityOptions>() is { } securityOptions:
                    Services.Configure<SecurityOptions>(section);

                    if (section.GetSection("BCrypt") is { } bCryptConfig &&
                        bCryptConfig.Get<BCryptOptions>() is { } bCryptOptions)
                    {
                        Services.Configure<BCryptOptions>(bCryptConfig);
                        this.AddBCryptPasswordHash(bCryptOptions);
                    }
                    
                    break;
            }
        
        if (configuration.GetSection("Domain:Users") is { } usersConfig)
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
    
    public DataAccessLibraryBuilder AddCloudStorage<T>() 
        where T : class, IStorageService
    {
        Services.AddTransient<IStorageService, T>();

        Services.AddTransient<IAvatarService, StorageEnabledAvatarService>();
        
        return this;
    }

    public DataAccessLibraryBuilder AddPasswordHasher<TPasswordHasher>() 
        where TPasswordHasher : class, IPasswordService
    {
        Services.AddSingleton<IPasswordService, TPasswordHasher>();

        return this;
    }

    public DataAccessLibraryBuilder AddDataProtection<TDataProtector>() where TDataProtector : class, IDataProtectionService
    {
        Services.AddTransient<IDataProtectionService, TDataProtector>();

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
        Services.AddSingleton<IIdService, Base64GuidIdService>();
        
        Services.AddTransient<IScopeValidator, UserRoleScopeValidator>();
        
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
}