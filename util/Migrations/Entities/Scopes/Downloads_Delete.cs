namespace Migrations.Entities.Scopes;

public class Downloads_Delete : EntityMigration<Scope>
{
    public Downloads_Delete() : base("scopes", new Scope
    {
        Id = "downloads.delete",
        Alias = "downloads.delete",
        Name = "downloads.delete",
        DisplayName = "Delete Downloads",
        Description = "Delete your downloads."
    })
    {
        
    }
}