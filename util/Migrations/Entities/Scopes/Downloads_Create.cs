namespace Migrations.Entities.Scopes;

public class Downloads_Create : EntityMigration<Scope>
{
    public Downloads_Create() : base("scopes", new Scope
    {
        Id = "downloads.create",
        Alias = "downloads.create",
        Name = "downloads.create",
        DisplayName = "Create Downloads",
        Description = "Create downloads with your account."
    })
    {
        
    }
}