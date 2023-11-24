namespace Migrations.Entities.Scopes;

public class Applications_Read : EntityMigration<Scope>
{
    public Applications_Read() : base("scopes", new Scope
    {
        Id = "applications.read",
        Alias = "applications.read",
        Name = "applications.read",
        DisplayName = "Read Applications",
        Description = "View your OAuth applications."
    })
    {
        
    }
}