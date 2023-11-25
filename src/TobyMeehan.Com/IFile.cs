namespace TobyMeehan.Com;

/// <summary>
/// A file in cloud storage.
/// </summary>
public interface IFile
{
    /// <summary>
    /// The user specified name of the file.
    /// </summary>
    string Filename { get; }
    
    /// <summary>
    /// The content type of the file.
    /// </summary>
    MediaType ContentType { get; }
    
    /// <summary>
    /// Size in bytes of the file.
    /// </summary>
    long Size { get; }
}