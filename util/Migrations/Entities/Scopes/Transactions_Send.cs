namespace Migrations.Entities.Scopes;

public class Transactions_Send : EntityMigration<Scope>
{
    public Transactions_Send() : base("scopes", new Scope
    {
        Id = "transactions.send",
        Alias = "transactions.send",
        Name = "transactions.send",
        DisplayName = "Send Transactions",
        Description = "Send transactions to alter your account balance."
    })
    {
        
    }
}