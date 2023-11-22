using System.Text.Json.Serialization;
using FastEndpoints;
using TobyMeehan.Com.Api.Requests;

namespace TobyMeehan.Com.Api.Features.Application.Update;

public class Request : AuthenticatedRequest
{
    public string Id { get; set; } = default!;
    
    public Optional<string> Name { get; set; }
    
    public Optional<string?> Description { get; set; }
}