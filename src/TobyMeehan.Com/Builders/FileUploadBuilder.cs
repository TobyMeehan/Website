namespace TobyMeehan.Com.Builders;

public struct FileUploadBuilder
{
    public FileUploadBuilder WithFileStream(Stream value) => this with { FileStream = value };
    public Stream FileStream { get; set; }

    public FileUploadBuilder WithFilename(string value) => this with { Filename = value };
    public string Filename { get; set; }

    public FileUploadBuilder WithContentType(MediaType value) => this with { ContentType = value };
    public MediaType ContentType { get; set; }

    public FileUploadBuilder WithSize(long value) => this with { Size = value };
    public long Size { get; set; }
}