using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;
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

    public DataAccessLibraryBuilder AddPostgresDatabase()
    {
        var config = Configuration?.GetSection("Postgres");

        if (config is null)
        {
            throw new Exception("No Postgres configuration provided.");
        }
        
        Services.AddTransient<IDbConnection>(_ => new NpgsqlConnection(config.GetConnectionString("Postgres")));

        return this;
    }
    
    public DataAccessLibraryBuilder AddSqlKataRepositories<TCompiler>() where TCompiler : Compiler, new()
    {
        Services.AddTransient<QueryFactory>(services =>
            new QueryFactory(services.GetRequiredService<IDbConnection>(), new TCompiler()));
        
        Services.AddTransient<IUserRepository, SqlKata.UserRepository>();
        Services.AddTransient<IApplicationRepository, SqlKata.ApplicationRepository>();

        return this;
    }

    public DataAccessLibraryBuilder AddEntityServices()
    {
        Services.AddTransient<IUserService, Services.UserService>();
        Services.AddTransient<IApplicationService, Services.ApplicationService>();

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
}