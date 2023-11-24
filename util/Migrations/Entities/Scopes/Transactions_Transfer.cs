namespace Migrations.Entities.Scopes;

public class Transactions_Transfer : EntityMigration<Scope>
{
    public Transactions_Transfer() : base("scopes", new Scope
    {
        Id = "transactions.transfer",
        Alias = "transactions.transfer",
        Name = "transactions.transfer",
        DisplayName = "Send Transfers",
        Description = "Transfer your account balance to other users."
    })
    {
        
    }
}