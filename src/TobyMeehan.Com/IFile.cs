namespace TobyMeehan.Com;

/// <summary>
/// A file in cloud storage.
/// </summary>
public interface IFile
{
    /// <summary>
    /// Object name of the file.
    /// </summary>
    string ObjectName { get; }
    
    /// <summary>
    /// Name of the bucket in which the file is stored.
    /// </summary>
    string BucketName { get; }
    
    /// <summary>
    /// Filename of the file.
    /// </summary>
    string Filename { get; }
    
    /// <summary>
    /// Size in bytes of the file.
    /// </summary>
    long Size { get; }
}