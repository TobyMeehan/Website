namespace TobyMeehan.Com.Builders;

public struct UpdateApplicationBuilder
{
    public UpdateApplicationBuilder WithDownload(Id<IDownload> value) => this with { Download = value };
    public Optional<Id<IDownload>?> Download { get; set; }

    public UpdateApplicationBuilder WithName(string value) => this with { Name = value };
    public Optional<string> Name { get; set; }

    public UpdateApplicationBuilder WithDescription(string? value) => this with { Description = value };
    public Optional<string?> Description { get; set; }

    public UpdateApplicationBuilder WithIcon(FileUploadBuilder value) => this with { Icon = value };
    public Optional<FileUploadBuilder> Icon { get; set; }
}