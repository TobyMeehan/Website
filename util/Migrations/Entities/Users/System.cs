using FluentMigrator;

namespace Migrations.Entities.Users;

public class System : EntityMigration<User>
{
    protected System() : base("users", new User
    {
        Id = "system",
        Username = "System",
        DisplayName = "System",
        HashedPassword = "password",
        Balance = double.MaxValue,
        Description = "System User"
    }) { }
}