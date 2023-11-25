using SqlKata;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Domain.Scopes.Models;

namespace TobyMeehan.Com.Data.Domain.Scopes.Repositories;

public class ScopeRepository : Repository<ScopeDto>, IScopeRepository
{
    public ScopeRepository(ISqlDataAccess db) : base(db, "scopes")
    {
    }

    private const string UserRoles = "userroles";
    private readonly Query _userRoles = new Query(UserRoles)
        .OrderBy("Name");

    private const string ScopeUserRoles = "scopes_userroles";
    private readonly Query _scopeUserRoles = new Query(ScopeUserRoles);
    
    protected override Query Query()
    {
        return base.Query()
            .OrderBy("Name")

            .LeftJoin(_scopeUserRoles.As(ScopeUserRoles), j => j.On($"{ScopeUserRoles}.ScopeId", $"{Table}.Id"))
            .LeftJoin(_userRoles.As(UserRoles), j => j.On($"{UserRoles}.Id", $"{ScopeUserRoles}.RoleId"))

            .Select($"{Table}.{{Id, Alias, Name, DisplayName, Description}}",
                $"{UserRoles}.Id AS UserRoles_Id", $"{UserRoles}.Name AS UserRoles_Name");
    }

    public async Task<ScopeDto?> SelectByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await Db.SingleAsync<ScopeDto>(Query()
                .Where(Column("Name"), name), 
            cancellationToken);
    }

    public async Task<ScopeDto?> SelectByAliasAsync(string alias, CancellationToken cancellationToken)
    {
        return await Db.SingleAsync<ScopeDto>(Query()
                .Where(Column("Alias"), alias),
            cancellationToken);
    }
}