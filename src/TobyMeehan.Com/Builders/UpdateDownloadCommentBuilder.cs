namespace TobyMeehan.Com.Builders;

public struct UpdateDownloadCommentBuilder
{
    public UpdateDownloadCommentBuilder WithContent(string value) => new() { Content = value };
    public Optional<string> Content { get; set; }
}