namespace Migrations.Entities;

public class User
{
    public required string Id { get; set; }
    public string? AvatarId { get; set; }
    public required string DisplayName { get; set; }
    public required string Username { get; set; }
    public required byte[] Password { get; set; }
    public required double Balance { get; set; }
    public string? Description { get; set; }
}