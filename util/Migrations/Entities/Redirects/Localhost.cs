using FluentMigrator;

namespace Migrations.Entities.Redirects;

public class Localhost : EntityMigration<Redirect>
{
    public Localhost() : base("redirects", new Redirect
    {
        Id = "localhost",
        ApplicationId = "system",
        Uri = "http://localhost:5700"
    })
    {
    }
}