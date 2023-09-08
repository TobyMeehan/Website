namespace TobyMeehan.Com.Data.Entities;

public class Redirect : Entity<IRedirect>, IRedirect
{
    public Redirect(string id, string applicationId, Uri uri) : base(id)
    {
        ApplicationId = new Id<IApplication>(applicationId);
        Uri = uri;
    }

    public Id<IApplication> ApplicationId { get; }
    public Uri Uri { get; }
}