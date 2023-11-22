namespace Migrations.Tables;

public partial class CreateTables
{
    protected void UpScopes()
    {
        Create.Table("scopes")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()
            
            .WithColumn("Alias")
            .AsString(Size.Name)
            .Indexed()

            .WithColumn("Name")
            .AsString(Size.Name)
            .Indexed()

            .WithColumn("DisplayName")
            .AsString(Size.Name)

            .WithColumn("Description")
            .AsString(Size.Medium);

        Create.Table("scopes_userroles")

            .WithColumn("ScopeId")
            .AsString(Size.Id)
            .PrimaryKey()
            .ForeignKey("scopes", "Id")

            .WithColumn("RoleId")
            .AsString(Size.Id)
            .PrimaryKey()
            .ForeignKey("userroles", "Id");
    }

    protected void DownScopes()
    {
        Delete.Table("scopes_userroles");
        Delete.Table("scopes");
    }
}