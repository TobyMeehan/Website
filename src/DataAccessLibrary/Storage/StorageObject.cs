namespace TobyMeehan.Com.Data.Storage;

public class StorageObject
{
    public required Guid ObjectName { get; init; }
    public required long Size { get; init; }
}