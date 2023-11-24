namespace Migrations.Entities.Scopes;

public class Applications_Delete : EntityMigration<Scope>
{
    public Applications_Delete() : base("scopes", new Scope
    {
        Id = "applications.delete",
        Alias = "applications.delete",
        Name = "applications.delete",
        DisplayName = "Delete Applications",
        Description = "Delete your OAuth applications."
    })
    {
        
    }
}