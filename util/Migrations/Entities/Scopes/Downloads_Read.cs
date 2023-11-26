namespace Migrations.Entities.Scopes;

public class Downloads_Read : EntityMigration<Scope>
{
    public Downloads_Read() : base("scopes", new Scope
    {
        Id = "downloads.read",
        Alias = "downloads.read",
        Name = "downloads.read",
        DisplayName = "View Downloads",
        Description = "View your unlisted and private downloads."
    })
    {
        
    }
}