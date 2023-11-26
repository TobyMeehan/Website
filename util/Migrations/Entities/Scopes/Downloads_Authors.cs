namespace Migrations.Entities.Scopes;

public class Downloads_Authors : EntityMigration<Scope>
{
    public Downloads_Authors() : base("scopes", new Scope
    {
        Id = "downloads.authors",
        Alias = "downloads.authors",
        Name = "downloads.authors",
        DisplayName = "Download Authors",
        Description = "Manage authors on your downloads."
    })
    {
        
    }
}