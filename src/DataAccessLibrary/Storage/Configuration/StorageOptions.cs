namespace TobyMeehan.Com.Data.Storage.Configuration;

public class StorageOptions
{
    public AvatarStorageOptions Avatars { get; set; } = new();
    public ApplicationIconStorageOptions ApplicationIcons { get; set; } = new();
}

public class AvatarStorageOptions
{
    public string Bucket { get; set; } = default!;
}

public class ApplicationIconStorageOptions
{
    public string Bucket { get; set; } = default!;
}