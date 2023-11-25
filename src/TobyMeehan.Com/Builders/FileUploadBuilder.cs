using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Builders;

public struct FileUploadBuilder : IFileUpload
{
    public FileUploadBuilder WithFilename(string value) => this with { Filename = value };
    public string Filename { get; set; }

    public FileUploadBuilder WithContentType(MediaType value) => this with { ContentType = value };
    public MediaType ContentType { get; set; }

    public FileUploadBuilder WithSize(long value) => this with { Size = value };
    public long Size { get; set; }

    public FileUploadBuilder WithStream(Stream value) => this with { Stream = value };
    public Stream Stream { get; set; }
}