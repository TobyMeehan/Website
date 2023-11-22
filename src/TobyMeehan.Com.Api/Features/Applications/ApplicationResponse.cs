namespace TobyMeehan.Com.Api.Features.Applications;

public class ApplicationResponse
{
    public required string Id { get; set; }
    public required string AuthorId { get; set; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
}