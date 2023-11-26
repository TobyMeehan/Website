using FluentMigrator;

namespace Migrations.Tables;

public class DownloadAuthors : Migration
{
    public override void Up()
    {
        Create.Table("downloadauthors")

            .WithColumn("DownloadId")
            .AsString(Size.Id)
            .PrimaryKey()
            .ForeignKey("downloads", "Id")

            .WithColumn("UserId")
            .AsString(Size.Id)
            .PrimaryKey()
            .ForeignKey("users", "Id")
            
            .WithColumn("CanEdit")
            .AsBoolean()
            
            .WithColumn("CanManageAuthors")
            .AsBoolean()
            
            .WithColumn("CanManageFiles")
            .AsBoolean()
            
            .WithColumn("CanDelete")
            .AsBoolean();
    }

    public override void Down()
    {
        Delete.Table("downloadauthors");
    }
}