namespace TobyMeehan.Com.Builders;

public struct CreateDownloadBuilder
{
    public CreateDownloadBuilder WithOwner(Id<IUser> value) => this with { Owner = value };
    public Id<IUser> Owner { get; set; }

    public CreateDownloadBuilder WithTitle(string value) => this with { Title = value };
    public string Title { get; set; }

    public CreateDownloadBuilder WithSummary(string value) => this with { Summary = value };
    public string Summary { get; set; }

    public CreateDownloadBuilder WithDescription(string value) => this with { Description = value };
    public string Description { get; set; }

    public CreateDownloadBuilder WithVisibility(Visibility value) => this with { Visibility = value };
    public Visibility Visibility { get; set; }

    public CreateDownloadBuilder WithVersion(Version value) => this with { Version = value };
    public Version Version { get; set; }
}