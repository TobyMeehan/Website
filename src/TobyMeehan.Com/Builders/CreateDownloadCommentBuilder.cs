namespace TobyMeehan.Com.Builders;

public struct CreateDownloadCommentBuilder
{
    public CreateDownloadCommentBuilder WithDownload(Id<IDownload> value) => this with { Download = value };
    public Id<IDownload> Download { get; set; }

    public CreateDownloadCommentBuilder WithAuthor(Id<IUser> value) => this with { Author = value };
    public Id<IUser> Author { get; set; }

    public CreateDownloadCommentBuilder WithContent(string value) => this with { Content = value };
    public string Content { get; set; }
}