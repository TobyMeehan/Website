namespace TobyMeehan.Com.Data
{
    public interface IFile
    {
        string DownloadLink { get; }
        string MediaLink { get; }
        string Filename { get; }
        MediaType ContentType { get; }
    }
}