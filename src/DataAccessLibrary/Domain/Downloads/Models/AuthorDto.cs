using SqlKata;

namespace TobyMeehan.Com.Data.Domain.Downloads.Models;

public class AuthorDto
{
    public required string DownloadId { get; set; }
    public required string UserId { get; set; }
    
    [Ignore]
    public string? Username { get; set; }
    [Ignore]
    public string? DisplayName { get; set; }
    
    public required bool CanEdit { get; set; }
    public required bool CanManageAuthors { get; set; }
    public required bool CanManageFiles { get; set; }
    public required bool CanDelete { get; set; }
}