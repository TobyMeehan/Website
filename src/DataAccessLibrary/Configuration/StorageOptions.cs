namespace TobyMeehan.Com.Data.Configuration;

public class StorageOptions
{
    public AvatarStorageOptions Avatars { get; set; } = new();
}

public class AvatarStorageOptions
{
    public string Bucket { get; set; } = default!;
}