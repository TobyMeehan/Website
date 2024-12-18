namespace TobyMeehan.Com.Domain.Thavyra;

public class User
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string ProfileUrl { get; init; }
    public required string AvatarUrl { get; init; }
}