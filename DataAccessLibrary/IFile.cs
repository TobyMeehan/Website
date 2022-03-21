namespace TobyMeehan.Com.Data
{
    public interface IFile
    {
        Id<IFile> Id { get; }
        
        string DownloadLink { get; }
        string MediaLink { get; }
    }
}