using FluentMigrator;

namespace Migrations.Tables;

public class UserRoles : Migration
{
    public override void Up()
    {
        Create.Table("userroles")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("Name")
            .AsString(Size.Name);

        Create.Table("users_userroles")

            .WithColumn("UserId")
            .AsString(Size.Id)
            .PrimaryKey()
            .ForeignKey("users", "Id")

            .WithColumn("RoleId")
            .AsString(Size.Id)
            .PrimaryKey()
            .ForeignKey("userroles", "Id");
    }

    public override void Down()
    {
        Delete.Table("users_userroles");
        Delete.Table("userroles");
    }
}