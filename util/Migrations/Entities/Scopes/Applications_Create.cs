namespace Migrations.Entities.Scopes;

public class Applications_Create : EntityMigration<Scope>
{
    public Applications_Create() : base("scopes", new Scope
    {
        Id = "applications.create",
        Alias = "applications.create",
        Name = "applications.create",
        DisplayName = "Create Applications",
        Description = "Add OAuth applications to your account."
    }) 
    {
        
    }
}