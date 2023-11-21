using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SqlKata.Compilers;
using TobyMeehan.Com.Data.Authorization;
using TobyMeehan.Com.Data.Caching;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Data.Security.BCrypt;
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
        Services.AddTransient<IApplicationRepository, SqlKata.ApplicationRepository>();
        Services.AddTransient<IAuthorizationRepository, SqlKata.AuthorizationRepository>();
        Services.AddTransient<IScopeRepository, SqlKata.ScopeRepository>();
        Services.AddTransient<ITokenRepository, SqlKata.TokenRepository>();
        Services.AddTransient<IUserRepository, SqlKata.UserRepository>();
        Services.AddTransient<IUserRoleRepository, SqlKata.UserRoleRepository>();

        Services.AddSingleton(typeof(ICacheService<,>), typeof(MemoryCacheService<,>));
        
        return this;
    }

    public DataAccessLibraryBuilder AddEntityServices()
    {
        Services.AddTransient<IApplicationService, Services.ApplicationService>();
        Services.AddTransient<IAuthorizationService, Services.AuthorizationService>();
        Services.AddTransient<IScopeService, Services.ScopeService>();
        Services.AddTransient<ITokenService, Services.TokenService>();
        Services.AddTransient<IUserService, Services.UserService>();
        Services.AddTransient<IUserRoleService, Services.UserRoleService>();
        
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