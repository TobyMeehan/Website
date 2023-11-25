using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SqlKata.Compilers;
using TobyMeehan.Com.Data.Configuration;

namespace TobyMeehan.Com.Data.DataAccess.Configuration;

public static class DataAccessLibraryBuilderExtensions
{
    public static DataAccessLibraryBuilder AddPostgresDatabase(this DataAccessLibraryBuilder builder, Action<PostgresOptions> configureOptions)
    {
        var options = new PostgresOptions();

        configureOptions(options);

        return AddPostgresDatabase(builder, options);
    }
    
    internal static DataAccessLibraryBuilder AddPostgresDatabase(this DataAccessLibraryBuilder builder, PostgresOptions options)
    {
        builder.Services.AddSingleton(NpgsqlDataSource.Create(options.ConnectionString ?? 
                                                      throw new ConfigurationException<PostgresOptions>(options, "Postgres connection string not provided.")));

        return builder.AddDatabase<NpgsqlConnectionFactory, PostgresCompiler, SqlDataAccess>();
    }
}