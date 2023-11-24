using FluentMigrator;

namespace Migrations.Tables;

public class Scopes : Migration
{
    public override void Up()
    {
        Create.Table("scopes")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()
            
            .WithColumn("Alias")
            .AsString(Size.Name)
            .Unique()

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

    public override void Down()
    {
        Delete.Table("scopes_userroles");
        Delete.Table("scopes");
    }
}