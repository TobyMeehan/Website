namespace Migrations.Entities.Scopes;

public class Transactions : EntityMigration<Scope>
{
    public Transactions() : base("scopes", new Scope
    {
        Id = "transactions",
        Alias = "transactions",
        Name = "transactions",
        DisplayName = "Transactions",
        Description = "View your transactions and alter your account balance."
    })
    {
        
    }
}