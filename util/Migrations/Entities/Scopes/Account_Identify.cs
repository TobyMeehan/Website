namespace Migrations.Entities.Scopes;

public class Account_Identify : EntityMigration<Scope>
{
    public Account_Identify() : base("scopes", new Scope
    {
        Id = "account.identify",
        Alias = "identify",
        Name = "account.identify",
        DisplayName = "Account Data",
        Description = "View your account balance and roles."
    })
    {
        
    }
}