namespace Migrations.Entities.Scopes;

public class Account_Update : EntityMigration<Scope>
{
    public Account_Update() : base("scopes", new Scope
    {
        Id = "account.update",
        Alias = "account.update",
        Name = "account.update",
        DisplayName = "Modify Account Data",
        Description = "Modify your account details and avatars."
    }){}
}