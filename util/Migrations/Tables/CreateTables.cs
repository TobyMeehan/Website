using FluentMigrator;

namespace Migrations.Tables;

[Migration(1)]
public partial class CreateTables : Migration
{
    public override void Up()
    {
        UpUsers();
        UpUserRoles();
        UpAvatars();
        
        UpApplications();
        UpRedirects();
        
        UpScopes();
        
        UpAuthorizations();
        UpTokens();
    }

    public override void Down()
    {
        DownTokens();
        DownAuthorizations();
        
        DownScopes();
        
        DownRedirects();
        DownApplications();
        
        DownAvatars();
        DownUserRoles();
        DownUsers();
    }
}