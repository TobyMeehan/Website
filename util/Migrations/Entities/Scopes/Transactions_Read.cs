namespace Migrations.Entities.Scopes;

public class Transactions_Read : EntityMigration<Scope>
{
    public Transactions_Read() : base("scopes", new Scope
    {
        Id = "transactions.read",
        Alias = "transactions.read",
        Name = "transactions.read",
        DisplayName = "Read Transactions",
        Description = "View transactions sent to your account."
    })
    {
        
    }
}