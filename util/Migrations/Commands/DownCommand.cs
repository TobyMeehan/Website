using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Migrations.Commands;

[Command("down", Description = "Downgrades database to a specified version.")]
public class DownCommand : ICommand
{
    [CommandOption("connectionString", 'c', Description = "Connection string for the database.")]
    public required string ConnectionString { get; set; }
    
    [CommandOption("version", 'v', Description = "Version to downgrade database to.")]
    public required long Version { get; set; }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        var runner = MigrationRunner.GetRunner(ConnectionString);

        runner.MigrateDown(Version);
        
        return default;
    }
}