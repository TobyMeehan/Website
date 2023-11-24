using FluentMigrator;

namespace Migrations.Tables;

public class Transactions : Migration
{
    public override void Up()
    {
        Create.Table("transactions")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("UserId")
            .AsString(Size.Id)
            .ForeignKey("users", "Id")
            .Indexed()

            .WithColumn("RecipientId")
            .AsString(Size.Id)
            .ForeignKey("users", "Id")
            .Indexed()
            .Nullable()

            .WithColumn("ApplicationId")
            .AsString(Size.Id)
            .ForeignKey("applications", "Id")

            .WithColumn("Description")
            .AsString(Size.Name)
            .Nullable()

            .WithColumn("Amount")
            .AsDouble()

            .WithColumn("SentAt")
            .AsDateTime()

            ;
    }

    public override void Down()
    {
        //Delete.Table("transactions");
    }
}