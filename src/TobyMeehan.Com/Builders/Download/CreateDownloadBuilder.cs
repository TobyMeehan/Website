using TobyMeehan.Com.Models.Download;

namespace TobyMeehan.Com.Builders.Download;

public struct CreateDownloadBuilder : ICreateDownload
{
    public CreateDownloadBuilder WithTitle(string value) => this with { Title = value };
    public string Title { get; set; }

    public CreateDownloadBuilder WithSummary(string value) => this with { Summary = value };
    public string Summary { get; set; }

    public CreateDownloadBuilder WithDescription(string? value) => this with { Description = value };
    public string? Description { get; set; }

    public CreateDownloadBuilder WithVisibility(string value) => this with { Visibility = value };
    public string Visibility { get; set; }

    public CreateDownloadBuilder WithUser(Id<IUser> value) => this with { User = value };
    public Id<IUser> User { get; set; }
}