namespace TobyMeehan.Com.Data.Authorization;

public class UserRoleScopeValidator : IScopeValidator
{
    public ValueTask<bool> CanValidateAsync(IScope scope, CancellationToken cancellationToken = default)
        => new(true);

    public ValueTask<bool> ValidateAsync(IScope scope, IUser user, IApplication application,
        CancellationToken cancellationToken = default)
    {
        return new ValueTask<bool>(
            !scope.UserRoles.Any(role => user.Roles[role.Id].IsNotFound()));
    }
}