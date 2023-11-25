using FluentMigrator;

namespace Migrations.Tables;

public class ApplicationIcons : Migration
{
    public override void Up()
    {
        Create.Table("applicationicons")

            .WithColumn("ApplicationId")
            .AsString(Size.Id)
            .PrimaryKey()
            .ForeignKey("applications", "Id")

            .WithColumn("Filename")
            .AsString(Size.Long)

            .WithColumn("ObjectName")
            .AsGuid()

            .WithColumn("ContentType")
            .AsString(Size.Short)

            .WithColumn("Size")
            .AsInt64()

            ;
    }

    public override void Down()
    {
        Delete.Table("applicationicons");
    }
}