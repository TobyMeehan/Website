namespace Migrations.Entities.Scopes;

public class Applications_Update : EntityMigration<Scope>
{
    public Applications_Update() : base("scopes", new Scope
    {
        Id = "applications.update",
        Alias = "applications.update",
        Name = "applications.update",
        DisplayName = "Update Applications",
        Description = "Modify your OAuth applications."
    })
    {
        
    }
}