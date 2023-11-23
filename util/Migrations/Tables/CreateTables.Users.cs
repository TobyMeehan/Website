namespace Migrations.Tables;

public partial class CreateTables
{
    protected void UpUsers()
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

    protected void DownUsers()
    {
        Delete.Table("users");
    }
}