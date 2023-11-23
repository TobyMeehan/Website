namespace TobyMeehan.Com.Api.Features.Avatars;

public class AvatarResponse
{
    public required Optional<string> Id { get; set; }
    public string Url => $"/users/avatar";
}