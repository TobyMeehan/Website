using FluentMigrator;

namespace Migrations.Entities.Redirects;

public class TobyMeehan : EntityMigration<Redirect>
{
    public TobyMeehan() : base("redirects", new Redirect
    {
        Id = "www.tobymeehan.com",
        ApplicationId = "system",
        Uri = "https://tobymeehan.com"
    })
    {
    }
}