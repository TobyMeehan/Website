using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Migrations;

public class MigrationRunner
{
    public static IMigrationRunner GetRunner(string connectionString) => new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false)
            .GetRequiredService<IMigrationRunner>();
    
}