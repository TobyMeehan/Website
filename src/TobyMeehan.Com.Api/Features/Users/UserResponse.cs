namespace TobyMeehan.Com.Api.Features.Users;

public class UserResponse
{
    public required Optional<string> Id { get; set; }
    public required Optional<string> Username { get; set; }
    public required Optional<string> DisplayName { get; set; }
    public required Optional<string?> Description { get; set; }
    public Optional<double> Balance { get; set; }
}