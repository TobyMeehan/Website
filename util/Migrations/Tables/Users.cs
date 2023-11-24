using FluentMigrator;

namespace Migrations.Tables;

public class Users : Migration
{
    public override void Up()
    {
        Create.Table("users")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()
            
            .WithColumn("AvatarId")
            .AsString(Size.Id)
            .Nullable()

            .WithColumn("Username")
            .AsString(Size.Name)
            .Unique()

            .WithColumn("DisplayName")
            .AsString(Size.Name)

            .WithColumn("HashedPassword")
            .AsString(Size.Short)

            .WithColumn("Balance")
            .AsDouble()

            .WithColumn("Description")
            .AsString(Size.Medium)
            .Nullable();
    }

    public override void Down()
    {
        Delete.Table("users");
    }
}