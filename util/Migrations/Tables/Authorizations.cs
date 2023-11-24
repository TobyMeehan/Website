using FluentMigrator;

namespace Migrations.Tables;

public class Authorizations : Migration
{
    public override void Up()
    {
        Create.Table("authorizations")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("ApplicationId")
            .AsString(Size.Id)
            .ForeignKey("applications", "Id")
            .Indexed()

            .WithColumn("UserId")
            .AsString(Size.Id)
            .ForeignKey("users", "Id")
            .Indexed()

            .WithColumn("Status")
            .AsString(Size.Short)
            .Nullable()

            .WithColumn("Type")
            .AsString(Size.Short)
            .Nullable()

            .WithColumn("CreatedAt")
            .AsDateTime()

            .WithColumn("Scopes")
            .AsString(Size.Long);
    }

    public override void Down()
    {
        Delete.Table("authorizations");
    }
}