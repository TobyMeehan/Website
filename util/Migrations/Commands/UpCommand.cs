using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Migrations.Commands;

[Command("up", Description = "Updates database to a specified version.")]
public class UpCommand : ICommand
{
    [CommandOption("connectionString", 'c', Description = "Connection string for the database.")]
    public required string ConnectionString { get; set; }

    [CommandOption("version", 'v', Description = "Version to update database to.")]
    public long? Version { get; set; }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        var runner = MigrationRunner.GetRunner(ConnectionString);

        if (Version.HasValue)
        {
            runner.MigrateUp(Version.Value);
        }
        else
        {
            runner.MigrateUp();
        }
        
        return default;
    }
}