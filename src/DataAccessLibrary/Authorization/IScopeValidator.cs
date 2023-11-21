namespace TobyMeehan.Com.Data.Authorization;

public interface IScopeValidator
{
    ValueTask<bool> CanValidateAsync(IScope scope, CancellationToken cancellationToken = default);

    ValueTask<bool> ValidateAsync(IScope scope, IUser user, IApplication application,
        CancellationToken cancellationToken = default);
}