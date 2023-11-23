namespace TobyMeehan.Com.Data.Entities;

public class Avatar : IAvatar
{
    public required Id<IAvatar> Id { get; init; }
    public required string Filename { get; init; }
    public required MediaType ContentType { get; init; }
    public required long Size { get; init; }
}