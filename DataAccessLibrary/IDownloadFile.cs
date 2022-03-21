namespace TobyMeehan.Com.Data
{
    public interface IDownloadFile : IFile
    {
        string Filename { get; }
    }
}