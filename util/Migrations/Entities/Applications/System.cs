using FluentMigrator;

namespace Migrations.Entities.Applications;

public class System : EntityMigration<Application>
{
    public System() : base("applications", new Application
    {
        Id = "system",
        AuthorId = "system",
        Name = "System",
        Description = "System Application"
    })
    {
    }

    public override void Down()
    {
        Delete.FromTable("tokens").AllRows();
        Delete.FromTable("authorizations").AllRows();
        
        base.Down();
    }
}