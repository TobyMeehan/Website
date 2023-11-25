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

            .WithColumn("Password")
            .AsBinary(Size.Short)

            .WithColumn("Balance")
            .AsDouble()

            .WithColumn("Description")
            .AsString(Size.Medium)
            .Nullable();
    }

    public override void Down()
    {
        Delete.FromTable("transactions")
            .Row(new { UserId = "system" })
            .Row(new {RecipientId = "system"});
        
        Delete.Table("users");
    }
}