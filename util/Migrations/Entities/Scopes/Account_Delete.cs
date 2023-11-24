namespace Migrations.Entities.Scopes;

public class Account_Delete : EntityMigration<Scope>
{
    public Account_Delete() : base("scopes", new Scope
    {
        Id = "account.delete",
        Alias = "account.delete",
        Name = "account.delete",
        DisplayName = "Delete Account",
        Description = "Delete your account."
    })
    {
        
    }
}