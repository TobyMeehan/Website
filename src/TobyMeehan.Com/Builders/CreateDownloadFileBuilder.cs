namespace TobyMeehan.Com.Builders;

public struct CreateDownloadFileBuilder
{
    public CreateDownloadFileBuilder WithDownload(Id<IDownload> value) => this with { Download = value };
    public Id<IDownload> Download { get; set; }

    public CreateDownloadFileBuilder WithFile(FileUploadBuilder value) => this with { File = value };
    public FileUploadBuilder File { get; set; }
}