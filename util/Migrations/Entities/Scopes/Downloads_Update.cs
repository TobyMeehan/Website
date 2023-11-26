namespace Migrations.Entities.Scopes;

public class Downloads_Update : EntityMigration<Scope>
{
    public Downloads_Update() : base("scopes", new Scope
    {
        Id = "downloads.update",
        Alias = "downloads.edit",
        Name = "downloads.update",
        DisplayName = "Edit Downloads",
        Description = "Edit your download pages."
    })
    {
        
    }
}