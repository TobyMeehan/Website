namespace TobyMeehan.Com.Api.Features.Applications;

public class ApplicationResponse
{
    public required Optional<string> Id { get; set; }
    public required Optional<string> AuthorId { get; set; }
    public required Optional<string> Name { get; set; }
    public required Optional<string?> Description { get; set; }
}