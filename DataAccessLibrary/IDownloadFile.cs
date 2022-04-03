namespace TobyMeehan.Com.Data
{
    public interface IDownloadFile : IFile
    {
        Id<IDownloadFile> Id { get; }
        string Filename { get; }
    }
}