namespace Migrations.Tables;

public partial class CreateTables
{
    protected void UpUserRoles()
    {
        Create.Table("userroles")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("Name")
            .AsString(Size.Name);

        Create.Table("users_userroles")

            .WithColumn("UserId")
            .AsString(Size.Id)
            .PrimaryKey()
            .ForeignKey("users", "Id")

            .WithColumn("RoleId")
            .AsString(Size.Id)
            .PrimaryKey()
            .ForeignKey("userroles", "Id");
    }

    protected void DownUserRoles()
    {
        Delete.Table("users_userroles");
        Delete.Table("userroles");
    }
}