using FluentMigrator;

namespace Migrations.Seeds;

[Migration(2)]
public class SystemEntities : Migration
{
    public override void Up()
    {
        Insert.IntoTable("users")
            .Row(new
            {
                Id = "system",
                Username = "System",
                DisplayName = "System",
                HashedPassword = "password",
                Balance = double.MaxValue,
                Description = "System User"
            });

        Insert.IntoTable("applications")
            .Row(new
            {
                Id = "system",
                AuthorId = "system",
                Name = "System",
                Description = "System Application"
            });

        Insert.IntoTable("redirects")
            .Row(new
            {
                Id = "www.tobymeehan.com",
                ApplicationId = "system",
                Uri = "https://tobymeehan.com"
            })
            .Row(new
            {
                Id = "localhost",
                ApplicationId = "system",
                Uri = "http://localhost:5700"
            });

        Insert.IntoTable("scopes")
            .Row(new
            {
                Id = "account",
                Alias = "account",
                Name = "account",
                DisplayName = "Account",
                Description = "View and manage your account details."
            })
            .Row(new
            {
                Id = "account.identify",
                Alias = "identify",
                Name = "account.identify",
                DisplayName = "Account Data",
                Description = "View your account balance and roles."
            })
            .Row(new
            {
                Id = "account.update",
                Alias = "account.update",
                Name = "account.update",
                DisplayName = "Modify Account Data",
                Description = "Modify your account details and avatars."
            })
            .Row(new
            {
                Id = "account.delete",
                Alias = "account.delete",
                Name = "account.delete",
                DisplayName = "Delete Account",
                Description = "Delete your account."
            })
            .Row(new
            {
                Id = "applications",
                Alias = "applications",
                Name = "applications",
                DisplayName = "Applications",
                Description = "Manage your OAuth applications."
            })
            .Row(new
            {
                Id = "applications.create",
                Alias = "applications.create",
                Name = "applications.create",
                DisplayName = "Create Applications",
                Description = "Add OAuth applications to your account."
            })
            .Row(new
            {
                Id = "applications.read",
                Alias = "applications.read",
                Name = "applications.read",
                DisplayName = "Read Applications",
                Description = "View your OAuth applications."
            })
            .Row(new
            {
                Id = "applications.update",
                Alias = "applications.update",
                Name = "applications.update",
                DisplayName = "Update Applications",
                Description = "Modify your OAuth applications."
            })
            .Row(new
            {
                Id = "applications.delete",
                Alias = "applications.delete",
                Name = "applications.delete",
                DisplayName = "Delete Applications",
                Description = "Delete your OAuth applications."
            });
    }

    public override void Down()
    {
        Delete.FromTable("tokens").AllRows();
        Delete.FromTable("authorizations").AllRows();
        Delete.FromTable("scopes").AllRows();
        
        Delete.FromTable("redirects").Row(new { ApplicationId = "system" });
        Delete.FromTable("applications").Row(new { Id = "system" });
        Delete.FromTable("users").Row(new { Id = "system" });
    }
}