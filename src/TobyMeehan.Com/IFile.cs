namespace TobyMeehan.Com;

/// <summary>
/// A file in cloud storage.
/// </summary>
public interface IFile
{
    /// <summary>
    /// The content type of the file.
    /// </summary>
    MediaType ContentType { get; }
    
    /// <summary>
    /// Size in bytes of the file.
    /// </summary>
    long Size { get; }
}