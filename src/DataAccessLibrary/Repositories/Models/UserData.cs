namespace TobyMeehan.Com.Data.Repositories.Models;

public class UserData
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public double Balance { get; set; }
    public string? Description { get; set; }
    public string? CustomUrl { get; set; }
}