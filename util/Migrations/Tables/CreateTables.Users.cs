namespace Migrations.Tables;

public partial class CreateTables
{
    protected void UpUsers()
    {
        Create.Table("users")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("Username")
            .AsString(Size.Name)
            .Indexed()

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