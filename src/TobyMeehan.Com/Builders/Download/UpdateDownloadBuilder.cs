using TobyMeehan.Com.Models.Download;

namespace TobyMeehan.Com.Builders.Download;

public struct UpdateDownloadBuilder : IUpdateDownload
{
    public UpdateDownloadBuilder WithTitle(string value) => this with { Title = value };
    public Optional<string> Title { get; set; }

    public UpdateDownloadBuilder WithSummary(string value) => this with { Summary = value };
    public Optional<string> Summary { get; set; }

    public UpdateDownloadBuilder WithDescription(string? value) => this with { Description = value };
    public Optional<string?> Description { get; set; }

    public UpdateDownloadBuilder WithVerification(string value) => this with { Verification = value };
    public Optional<string> Verification { get; set; }

    public UpdateDownloadBuilder WithVisibility(string value) => this with { Visibility = value };
    public Optional<string> Visibility { get; set; }
}