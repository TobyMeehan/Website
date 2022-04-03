using System.IO;

namespace TobyMeehan.Com.Data;

public class FileUpload
{
    public string Filename { get; set; }
    public MediaType ContentType { get; set; }
    public Stream UploadStream { get; set; }
}