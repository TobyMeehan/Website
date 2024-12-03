namespace TobyMeehan.Com.Features.Uploads.Complete;

public class Request : UploadRequest
{
    public List<Part> Parts { get; set; } = [];
    
    public class Part
    {
        public int PartNumber { get; set; }
        public long Size { get; set; }
        public string ETag { get; set; }
    }
}