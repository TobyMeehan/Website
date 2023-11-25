using System.Text.Json.Serialization;

namespace TobyMeehan.Com.Api.Features.Avatars;

public class AvatarResponse
{
    public required string Id { get; set; }

    [JsonIgnore] public string UserId { get; init; } = default!;
    [JsonIgnore] public MediaType ContentType { get; init; }
    
    public string Url => $"/users/{UserId}/avatars/{Id}/avatar{ContentType.Extension}";
}