namespace Migrations.Entities.Scopes;

public class Applications : EntityMigration<Scope>
{
    public Applications() : base("scopes", new Scope
    {
        Id = "applications",
        Alias = "applications",
        Name = "applications",
        DisplayName = "Applications",
        Description = "Manage your OAuth applications."
    })
    {
        
    }
}