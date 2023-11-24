namespace TobyMeehan.Com.Api.Security;

public class Resource<T>
{
    public IUser User { get; }

    public Resource(IUser user)
    {
        User = user;
    }
}