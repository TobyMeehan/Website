using FluentMigrator;

namespace Migrations.Entities.Scopes;

public class Downloads : EntityMigration<Scope>
{
    public Downloads() : base("scopes", new Scope
    {
        Id = "downloads",
        Alias = "downloads",
        Name = "downloads",
        DisplayName = "Downloads",
        Description = "Create, delete and manage your downloads, files and authors."
    })
    {
    }
}