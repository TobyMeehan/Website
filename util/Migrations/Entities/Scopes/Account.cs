namespace Migrations.Entities.Scopes;

public class Account : EntityMigration<Scope>
{
    public Account() : base("scopes", new Scope
    {
        Id = "account",
        Alias = "account",
        Name = "account",
        DisplayName = "Account",
        Description = "View and manage your account details."
    })
    {
    }
}