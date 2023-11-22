namespace Migrations.Tables;

public partial class CreateTables
{
    protected void UpRedirects()
    {
        Create.Table("redirects")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("ApplicationId")
            .AsString(Size.Id)
            .ForeignKey("applications", "Id")
            .Indexed()

            .WithColumn("Uri")
            .AsString(1000)
            .Indexed();
    }

    protected void DownRedirects()
    {
        Delete.Table("redirects");
    }
}