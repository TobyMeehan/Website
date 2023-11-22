namespace Migrations.Tables;

public partial class CreateTables
{
    protected void UpApplications()
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
            
            .WithColumn("SecretHash")
            .AsString(Size.Short)
            .Nullable();
    }

    protected void DownApplications()
    {
        Delete.Table("applications");
    }
}