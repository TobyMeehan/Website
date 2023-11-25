using FluentMigrator;

namespace Migrations.Tables;

public class Applications : Migration
{
    public override void Up()
    {
        Create.Table("applications")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("AuthorId")
            .AsString(Size.Id)
            .ForeignKey("users", "Id")
            .Indexed()

            .WithColumn("DownloadId")
            .AsString(Size.Id)
            .Nullable()

            .WithColumn("Name")
            .AsString(Size.Name)

            .WithColumn("Description")
            .AsString(Size.Medium)
            .Nullable()
            
            .WithColumn("Secret")
            .AsBinary(Size.Short)
            .Nullable();
    }

    public override void Down()
    {
        Delete.Table("applications");
    }
}