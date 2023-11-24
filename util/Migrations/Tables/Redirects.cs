using FluentMigrator;

namespace Migrations.Tables;

public class Redirects : Migration
{
    public override void Up()
    {
        Create.Table("redirects")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("ApplicationId")
            .AsString(Size.Id)
            .ForeignKey("applications", "Id")
            .Indexed()

            .WithColumn("Uri")
            .AsString(1000)
            .Indexed();
    }

    public override void Down()
    {
        Delete.Table("redirects");
    }
}