using FluentMigrator;

namespace Migrations.Tables;

public class Downloads : Migration
{
    public override void Up()
    {
        Create.Table("downloads")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("Title")
            .AsString(Size.Name)

            .WithColumn("Summary")
            .AsString(Size.Medium)

            .WithColumn("Description")
            .AsString(Size.Medium)
            .Nullable()

            .WithColumn("Verification")
            .AsString(Size.Short)

            .WithColumn("Visibility")
            .AsString(Size.Short)
            
            .WithColumn("UpdatedAt")
            .AsDateTime();
    }

    public override void Down()
    {
        Delete.Table("downloads");
    }
}