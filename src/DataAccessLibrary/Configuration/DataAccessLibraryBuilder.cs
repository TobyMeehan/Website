using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SqlKata.Compilers;
using TobyMeehan.Com.Data.Authorization;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.DataAccess;
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
        Configuration = configuration;
    }
    
    public IServiceCollection Services { get; }
    public IConfiguration? Configuration { get; }

    public DataAccessLibraryBuilder AddPostgresDatabase(string connectionStringName = "Default")
    {
        var config = Configuration?.GetSection("Postgres") ??
                     throw new ConfigurationException(Configuration, "Postgres");

        string connectionString = config.GetConnectionString(connectionStringName) ??
                     throw new ConfigurationException(Configuration, $"ConnectionStrings:{connectionStringName}");

        Services.AddSingleton(NpgsqlDataSource.Create(connectionString));
        
        Services.AddSingleton<IDbConnectionFactory, PostgresConnectionFactory>();
        Services.AddSingleton<Compiler, PostgresCompiler>();
        
        Services.AddTransient<ISqlDataAccess, SqlDataAccess>();
        
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

        Services.AddSingleton(typeof(ICacheService<,>), typeof(MemoryCacheService<,>));
        
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
        
        return this;
    }

    public DataAccessLibraryBuilder AddCloudStorage<T>() where T : class, IStorageService
    {
        var config = Configuration?.GetSection("Storage") ??
                     throw new ConfigurationException(Configuration, "Storage");
        
        Services.AddTransient<IStorageService, T>();
        Services.Configure<StorageOptions>(config);

        Services.AddTransient<IAvatarService, StorageEnabledAvatarService>();
        
        return this;
    }
    
    public DataAccessLibraryBuilder AddGoogleCloudStorage()
    {
        var credential = Configuration?.GetSection("Storage:Google") switch
        {
            { } configuration when configuration["CredentialJson"] is { } section =>
                GoogleCredential.FromJson(section),

            { } configuration when configuration.GetSection("Credential") is { } section =>
                GoogleCredential.FromJsonParameters(section.Get<JsonCredentialParameters>()),

            _ => throw new ConfigurationException(Configuration, "Storage:Google")
        };

        Services.AddSingleton(credential);
        
        Services.AddTransient(services => 
            StorageClient.Create(services.GetRequiredService<GoogleCredential>()));
        
        return AddCloudStorage<GoogleStorageService>();
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