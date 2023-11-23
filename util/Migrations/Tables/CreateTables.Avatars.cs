namespace Migrations.Tables;

public partial class CreateTables
{
    protected void UpAvatars()
    {
        Create.Table("avatars")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("UserId")
            .AsString(Size.Id)
            .ForeignKey("users", "Id")
            .Indexed()

            .WithColumn("ObjectName")
            .AsGuid()

            .WithColumn("Filename")
            .AsString(Size.Long)

            .WithColumn("ContentType")
            .AsString(Size.Short)

            .WithColumn("Size")
            .AsInt64();
    }

    protected void DownAvatars()
    {
        Delete.Table("avatars");
    }
}