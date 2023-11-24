using FluentMigrator;

namespace Migrations.Tables;

public class Avatars : Migration
{
    public override void Up()
    {
        Create.Table("avatars")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("UserId")
            .AsString(Size.Id)
            .ForeignKey("users", "Id")
            .Indexed()

            .WithColumn("ObjectName")
            .AsGuid()

            .WithColumn("Filename")
            .AsString(Size.Long)

            .WithColumn("ContentType")
            .AsString(Size.Short)

            .WithColumn("Size")
            .AsInt64();
    }

    public override void Down()
    {
        Delete.Table("avatars");
    }
}