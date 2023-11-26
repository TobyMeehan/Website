namespace Migrations.Entities.Scopes;

public class Downloads_Files : EntityMigration<Scope>
{
    public Downloads_Files() : base("scopes", new Scope
    {
        Id = "downloads.files",
        Alias = "downloads.files",
        Name = "downloads.files",
        DisplayName = "Download Files",
        Description = "Manage your downloads' files."
    })
    {
        
    }
}